using Microsoft.EntityFrameworkCore;
using Turnament.Models;

namespace Turnament.Data;

public static class DbInitializer
{
    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public static void Initialize(AppDbContext context)
    {
    try 
    {
        Console.WriteLine("Rozpoczynam seedowanie bazy...");
        context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            Console.WriteLine("Baza już zawiera dane - przerywam");
            return;
        }

        // Dodawanie sportów
        Console.WriteLine("Dodawanie sportów...");
        var sports = new List<Sport>
        {
            new() { Name = "Piłka nożna" },
            new() { Name = "Siatkówka" },
            new() { Name = "Koszykówka" },
            new() { Name = "E-sport" },
            new() { Name = "Tenis stołowy" },
            new() { Name = "Szachy" },
            new() { Name = "Piłka ręczna" },
            new() { Name = "Badminton" },
            new() { Name = "Counter-Strike" },
            new() { Name = "League of Legends" }
        };
        context.Sports.AddRange(sports);
        context.SaveChanges();
        Console.WriteLine($"Dodano {sports.Count} sportów");

        // Dodawanie typów turniejów
        Console.WriteLine("Dodawanie typów turniejów...");
        var bracketTypes = new List<BracketType>
        {
            new() { Name = "Pucharowy" },
            new() { Name = "Ligowy" },
            new() { Name = "Grupowy" },
            new() { Name = "Podwójnej eliminacji" },
            new() { Name = "Szwajcarski" },
            new() { Name = "Round Robin" }
        };

        context.BracketTypes.AddRange(bracketTypes);
        context.SaveChanges();

        // Dodawanie użytkowników
        Console.WriteLine("Dodawanie użytkowników...");
        var users = new List<User>
        {
            new() { Username = "admin", Email = "admin@turniej.pl", PassHash = HashPassword("Admin123!@#"), CreatedAt = DateTime.Now.AddYears(-2) },
            new() { Username = "jan_kowalski", Email = "jan@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-23) },
            new() { Username = "anna_nowak", Email = "anna@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-22) },
            new() { Username = "marek_wisniewski", Email = "marek@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-21) },
            new() { Username = "zofia_lewandowska", Email = "zofia@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-20) },
            new() { Username = "piotr_wojcik", Email = "piotr@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-19) },
            new() { Username = "ewa_kaminska", Email = "ewa@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-18) },
            new() { Username = "tomasz_zielinski", Email = "tomasz@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-17) },
            new() { Username = "magdalena_szymanska", Email = "magdalena@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-16) },
            new() { Username = "krzysztof_dabrowski", Email = "krzysztof@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-15) },
            new() { Username = "karolina_witkowska", Email = "karolina@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-14) },
            new() { Username = "michal_kowalczyk", Email = "michal@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-13) },
            new() { Username = "agnieszka_wojcik", Email = "agnieszka@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-12) },
            new() { Username = "pawel_krawczyk", Email = "pawel@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-11) },
            new() { Username = "monika_piotrowska", Email = "monika@example.com", PassHash = HashPassword("User123!@#"), CreatedAt = DateTime.Now.AddMonths(-10) }
        };


        context.Users.AddRange(users);
        context.SaveChanges();

        // Dodawanie drużyn
        Console.WriteLine("Dodawanie drużyn...");
        var teams = new List<Team>
        {
            new() { Name = "Orły Poznań", CreatorId = users[0].Id, CreatedAt = DateTime.Now.AddMonths(-24) },
            new() { Name = "Tygrysy Warszawa", CreatorId = users[1].Id, CreatedAt = DateTime.Now.AddMonths(-23) },
            new() { Name = "Wilki Kraków", CreatorId = users[2].Id, CreatedAt = DateTime.Now.AddMonths(-22) },
            new() { Name = "Lwy Wrocław", CreatorId = users[3].Id, CreatedAt = DateTime.Now.AddMonths(-21) },
            new() { Name = "Sokoły Gdańsk", CreatorId = users[4].Id, CreatedAt = DateTime.Now.AddMonths(-20) },
            new() { Name = "Pantery Łódź", CreatorId = users[5].Id, CreatedAt = DateTime.Now.AddMonths(-19) },
            new() { Name = "Niedźwiedzie Katowice", CreatorId = users[6].Id, CreatedAt = DateTime.Now.AddMonths(-18) },
            new() { Name = "Jastrzębie Szczecin", CreatorId = users[7].Id, CreatedAt = DateTime.Now.AddMonths(-17) },
            new() { Name = "Dragons Gaming", CreatorId = users[8].Id, CreatedAt = DateTime.Now.AddMonths(-16) },
            new() { Name = "Phoenix Esports", CreatorId = users[9].Id, CreatedAt = DateTime.Now.AddMonths(-15) },
            new() { Name = "Team Infinity", CreatorId = users[10].Id, CreatedAt = DateTime.Now.AddMonths(-14) },
            new() { Name = "Victory Squad", CreatorId = users[11].Id, CreatedAt = DateTime.Now.AddMonths(-13) },
            new() { Name = "Elite Warriors", CreatorId = users[12].Id, CreatedAt = DateTime.Now.AddMonths(-12) },
            new() { Name = "Legendary Team", CreatorId = users[13].Id, CreatedAt = DateTime.Now.AddMonths(-11) },
            new() { Name = "Supreme Gaming", CreatorId = users[14].Id, CreatedAt = DateTime.Now.AddMonths(-10) }
        };

        context.Teams.AddRange(teams);
        context.SaveChanges();

        // Dodawanie członków drużyn
        Console.WriteLine("Dodawanie członków drużyn...");
        var teamMembers = new List<TeamMember>();
        var random = new Random();

        foreach (var team in teams)
        {
            Console.WriteLine($"Dodawanie członków do drużyny: {team.Name}");
            var memberCount = random.Next(3, 6);
            var availableUsers = users.Where(u => u.Id != team.CreatorId).ToList();
            Console.WriteLine($"Dostępnych użytkowników: {availableUsers.Count}");

            for (var i = 0; i < memberCount && availableUsers.Any(); i++)
            {
                var randomIndex = random.Next(availableUsers.Count);
                var selectedUser = availableUsers[randomIndex];
                
                try 
                {
                    teamMembers.Add(new TeamMember 
                    { 
                        TeamId = team.Id, 
                        UserId = selectedUser.Id,
                        Role = "member",
                        CreatedAt = DateTime.Now
                    });
                    Console.WriteLine($"Dodano użytkownika {selectedUser.Username} do drużyny {team.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas dodawania członka drużyny: {ex.Message}");
                }
                
                availableUsers.RemoveAt(randomIndex);
            }
        }

        try 
        {
            context.TeamMembers.AddRange(teamMembers);
            context.SaveChanges();
            Console.WriteLine($"Pomyślnie dodano {teamMembers.Count} członków drużyn");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas zapisywania członków drużyn: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }

        // Turnieje
        var tournaments = new List<Tournament>
        {
            new()
            {
                Name = "Wiosenny Turniej Piłkarski 2024",
                Description = "Największy amatorski turniej piłkarski w regionie",
                CreatorId = users[0].Id,
                SportId = sports[0].Id,
                BracketTypeId = bracketTypes[0].Id,
                StartDate = DateTime.Now.AddDays(30),
                EndDate = DateTime.Now.AddDays(35)
            },
            new()
            {
                Name = "Wielkopolska Liga Siatkówki",
                Description = "Profesjonalna liga siatkówki dla zespołów z Wielkopolski",
                CreatorId = users[1].Id,
                SportId = sports[1].Id,
                BracketTypeId = bracketTypes[1].Id,
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(45)
            },
            new()
            {
                Name = "Puchar Koszykówki 2024",
                Description = "Elitarny turniej koszykówki dla najlepszych drużyn",
                CreatorId = users[2].Id,
                SportId = sports[2].Id,
                BracketTypeId = bracketTypes[0].Id,
                StartDate = DateTime.Now.AddDays(20),
                EndDate = DateTime.Now.AddDays(25)
            },
            new()
            {
                Name = "CS:GO Masters",
                Description = "Prestiżowy turniej Counter-Strike: Global Offensive",
                CreatorId = users[3].Id,
                SportId = sports[8].Id,
                BracketTypeId = bracketTypes[3].Id,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(12)
            },
            new()
            {
                Name = "Liga Legend 2024",
                Description = "Sezonowy turniej League of Legends",
                CreatorId = users[4].Id,
                SportId = sports[9].Id,
                BracketTypeId = bracketTypes[2].Id,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(40)
            },
            new()
            {
                Name = "Turniej Tenisa Stołowego",
                Description = "Amatorski turniej tenisa stołowego",
                CreatorId = users[5].Id,
                SportId = sports[4].Id,
                BracketTypeId = bracketTypes[0].Id,
                StartDate = DateTime.Now.AddDays(25),
                EndDate = DateTime.Now.AddDays(27)
            },
            new()
            {
                Name = "Szachowe Mistrzostwa",
                Description = "Turniej szachowy w systemie szwajcarskim",
                CreatorId = users[6].Id,
                SportId = sports[5].Id,
                BracketTypeId = bracketTypes[4].Id,
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(17)
            }
        };

        context.Tournaments.AddRange(tournaments);
        context.SaveChanges();

        // Drużyny w turniejach
        var tournamentTeams = new List<TournamentTeam>();

        foreach (var tournament in tournaments)
        {
            // Dodaj 4-8 losowych drużyn do każdego turnieju
            var teamCount = random.Next(4, 9);
            var availableTeams = new List<Team>(teams);

            for (var i = 0; i < teamCount && availableTeams.Any(); i++)
            {
                var randomIndex = random.Next(availableTeams.Count);
                var selectedTeam = availableTeams[randomIndex];

                tournamentTeams.Add(new TournamentTeam { TournamentId = tournament.Id, TeamId = selectedTeam.Id });
                availableTeams.RemoveAt(randomIndex);
            }
        }

        context.TournamentTeams.AddRange(tournamentTeams);
        context.SaveChanges();

        Console.WriteLine("Seedowanie zakończone pomyślnie");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd podczas seedowania: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}
}