using Turnament.Data;
using Turnament.Models;
using Microsoft.EntityFrameworkCore;

namespace Turnament.Services
{
    public class TournamentBracketService
    {
        private readonly AppDbContext _context;

        public TournamentBracketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateBracketAsync(int tournamentId)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentTeams)
                .ThenInclude(t => t.Team)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje");

            // Pobierz wszystkie drużyny z turnieju i wymieszaj ich kolejność
            var teams = tournament.TournamentTeams.ToList();
            var randomizedTeams = RandomizeTeams(teams);
        
            // Oblicz liczbę rund na podstawie liczby drużyn
            var numberOfTeams = teams.Count;
            var numberOfRounds = CalculateNumberOfRounds(numberOfTeams);
        
            // Generuj mecze pierwszej rundy
            var firstRoundMatches = await GenerateFirstRoundMatchesAsync(tournamentId, randomizedTeams);
        
            // Generuj puste mecze dla kolejnych rund
            await GenerateEmptyMatchesForNextRoundsAsync(tournamentId, numberOfRounds, teams.Count);
        }

        private int CalculateNumberOfRounds(int numberOfTeams)
        {
            return (int)Math.Ceiling(Math.Log(numberOfTeams, 2));
        }

        private List<TournamentTeam> RandomizeTeams(List<TournamentTeam> teams)
        {
            var rng = new Random();
            return teams.OrderBy(t => rng.Next()).ToList();
        }

        private async Task<List<Match>> GenerateFirstRoundMatchesAsync(int tournamentId, List<TournamentTeam> teams)
        {
            var matches = new List<Match>();
            var startDate = DateTime.UtcNow.Date.AddDays(1);

            for (int i = 0; i < teams.Count; i += 2)
            {
                var match = new Match
                {
                    TournamentId = tournamentId,
                    Round = 1,
                    Team1Id = teams[i].Id,
                    Team2Id = i + 1 < teams.Count ? teams[i + 1].Id : null,
                    ScheduledAt = startDate
                };
            
                matches.Add(match);
            }

            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        
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
                        ScheduledAt = startDate.AddDays(round - 1)
                    };
                    matches.Add(match);
                }

                await _context.Matches.AddRangeAsync(matches);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateMatchResultAsync(int matchId, int winnerId)
        {
            var match = await _context.Matches
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new ArgumentException("Mecz nie istnieje");

            // Aktualizuj wynik bieżącego meczu
            match.WinnerId = winnerId;
            match.FinishedAt = DateTime.UtcNow;

            // Znajdź następny mecz w drabince
            await AdvanceWinnerToNextRoundAsync(match);
        
            await _context.SaveChangesAsync();
        }

        private async Task AdvanceWinnerToNextRoundAsync(Match currentMatch)
        {
            // Znajdź wszystkie mecze w bieżącej rundzie
            var matchesInCurrentRound = await _context.Matches
                .Where(m => m.TournamentId == currentMatch.TournamentId && m.Round == currentMatch.Round)
                .OrderBy(m => m.Id)
                .ToListAsync();

            // Oblicz indeks następnego meczu
            var currentMatchIndex = matchesInCurrentRound.IndexOf(currentMatch);
            var nextRoundMatchIndex = currentMatchIndex / 2;

            // Znajdź następny mecz
            var nextMatch = await _context.Matches
                .FirstOrDefaultAsync(m => 
                    m.TournamentId == currentMatch.TournamentId && 
                    m.Round == currentMatch.Round + 1 &&
                    (m.Team1Id == null || m.Team2Id == null));

            if (nextMatch != null)
            {
                if (nextMatch.Team1Id == null)
                    nextMatch.Team1Id = currentMatch.WinnerId;
                else
                    nextMatch.Team2Id = currentMatch.WinnerId;
            }
        }
    }
}