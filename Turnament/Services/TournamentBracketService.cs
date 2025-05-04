using Turnament.Data;
using Turnament.Models;
using Microsoft.EntityFrameworkCore;

namespace Turnament.Services
{
    public class TournamentBracketService(AppDbContext context)
    {
        public async Task GenerateBracketAsync(int tournamentId)
        {
            var tournament = await context.Tournaments
                .Include(t => t.TournamentTeams)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje");

            // Usuń istniejące mecze dla tego turnieju
            await ClearExistingBracketAsync(tournamentId);

            // Pobierz wszystkie drużyny z turnieju i wymieszaj ich kolejność
            var teams = tournament.TournamentTeams.ToList();
            var randomizedTeams = RandomizeTeams(teams);
            
            // Oblicz liczbę rund
            var numberOfTeams = teams.Count;
            var numberOfRounds = CalculateNumberOfRounds(numberOfTeams);
            
            // Generuj mecze pierwszej rundy
            var firstRoundMatches = await GenerateFirstRoundMatchesAsync(tournamentId, randomizedTeams);
            
            // Generuj puste mecze dla kolejnych rund
            await GenerateEmptyMatchesForNextRoundsAsync(tournamentId, numberOfRounds, teams.Count);
        }

        private async Task ClearExistingBracketAsync(int tournamentId)
        {
            var existingMatches = await context.Matches
                .Where(m => m.TournamentId == tournamentId)
                .ToListAsync();

            if (existingMatches.Any())
            {
                context.Matches.RemoveRange(existingMatches);
                await context.SaveChangesAsync();
            }
        }

        private int CalculateNumberOfRounds(int numberOfTeams)
        {
            return (int)Math.Ceiling(Math.Log(numberOfTeams, 2));
        }

        private List<TournamentTeam> RandomizeTeams(List<TournamentTeam> teams)
        {
            // Sprawdź czy wszystkie mecze w turnieju są puste (nowa drabinka)
            var rng = new Random();
            return teams.OrderBy(t => rng.Next()).ToList();
        }

        private async Task<List<Match>> GenerateFirstRoundMatchesAsync(int tournamentId, List<TournamentTeam> teams)
        {
            var matches = new List<Match>();
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var matchesNeeded = (int)Math.Pow(2, Math.Ceiling(Math.Log2(teams.Count)));
            var byes = matchesNeeded - teams.Count; // liczba wolnych miejsc

            for (int i = 0; i < matchesNeeded / 2; i++)
            {
                var match = new Match
                {
                    TournamentId = tournamentId,
                    Round = 1,
                    ScheduledAt = startDate.AddHours(i * 2), // Dodajemy 2 godziny między meczami
                };

                // Obsługa "bye" - wolnych miejsc
                if (i < teams.Count / 2)
                {
                    match.Team1Id = teams[i * 2].Id;
                    if ((i * 2 + 1) < teams.Count)
                    {
                        match.Team2Id = teams[i * 2 + 1].Id;
                    }
                    else
                    {
                        // Automatycznie awansuj Team1 do następnej rundy jeśli nie ma przeciwnika
                        match.WinnerId = match.Team1Id;
                        match.FinishedAt = DateTime.UtcNow;
                    }
                }

                matches.Add(match);
            }

            await context.Matches.AddRangeAsync(matches);
            await context.SaveChangesAsync();
            
            // Jeśli są mecze z automatycznym awansem, od razu zaktualizuj następną rundę
            var autoAdvanceMatches = matches.Where(m => m.WinnerId.HasValue).ToList();
            foreach (var match in autoAdvanceMatches)
            {
                await AdvanceWinnerToNextRoundAsync(match);
            }
            
            if (autoAdvanceMatches.Any())
            {
                await context.SaveChangesAsync();
            }

            return matches;
        }

        private async Task GenerateEmptyMatchesForNextRoundsAsync(int tournamentId, int totalRounds, int numberOfTeams)
        {
            var matchesInRound = numberOfTeams / 2;
            var startDate = DateTime.UtcNow.Date.AddDays(1);

            for (int round = 2; round <= totalRounds; round++)
            {
                matchesInRound = matchesInRound / 2;
                var matches = new List<Match>();

                for (int i = 0; i < matchesInRound; i++)
                {
                    var match = new Match
                    {
                        TournamentId = tournamentId,
                        Round = round,
                        ScheduledAt = startDate.AddDays(round - 1).AddHours(i * 2)
                    };
                    matches.Add(match);
                }

                await context.Matches.AddRangeAsync(matches);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateMatchResultAsync(int matchId, int winnerId)
        {
            var match = await context.Matches
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new ArgumentException("Mecz nie istnieje");

            // Sprawdź, czy zwycięzca był uczestnikiem meczu
            if (winnerId != match.Team1Id && winnerId != match.Team2Id)
                throw new ArgumentException("Nieprawidłowa drużyna zwycięzcy");

            // Aktualizuj wynik bieżącego meczu
            match.WinnerId = winnerId;
            match.FinishedAt = DateTime.UtcNow;

            // Przenieś zwycięzcę do następnej rundy
            await AdvanceWinnerToNextRoundAsync(match);
            
            await context.SaveChangesAsync();
        }

        private async Task AdvanceWinnerToNextRoundAsync(Match currentMatch)
        {
            if (!currentMatch.WinnerId.HasValue)
                return;

            var matchesInCurrentRound = await context.Matches
                .Where(m => m.TournamentId == currentMatch.TournamentId && m.Round == currentMatch.Round)
                .OrderBy(m => m.Id)
                .ToListAsync();

            var currentMatchIndex = matchesInCurrentRound.IndexOf(currentMatch);
            var nextRoundMatchIndex = currentMatchIndex / 2;

            var nextMatch = await context.Matches
                .FirstOrDefaultAsync(m => 
                    m.TournamentId == currentMatch.TournamentId && 
                    m.Round == currentMatch.Round + 1 &&
                    (m.Team1Id == null || m.Team2Id == null));

            if (nextMatch != null)
            {
                if (nextMatch.Team1Id == null)
                {
                    nextMatch.Team1Id = currentMatch.WinnerId;
                }
                else
                {
                    nextMatch.Team2Id = currentMatch.WinnerId;
                }
            }
        }
    }
}