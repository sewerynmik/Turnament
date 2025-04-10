using Microsoft.EntityFrameworkCore;
using Turnament.Models;

namespace Turnament.Data;

public class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate(); // Upewnij się, że migracje zostały zastosowane

        // Sprawdź, czy tabela 'Users' jest pusta
        if (!context.Users.Any())
        {
            var user = new User
            {
                Username = "user",
                Email = "user@mail.com",
                PassHash = BCrypt.Net.BCrypt.HashPassword("pass"), // Hasło powinno być zahashowane
                CreatedAt = DateTime.Now
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Sprawdź, czy tabela 'Sports' jest pusta
        if (!context.Sports.Any())
        {
            context.Sports.AddRange(
                new Sport { Name = "Football" },
                new Sport { Name = "Basketball" },
                new Sport { Name = "Tennis" }
            );
            context.SaveChanges();
        }

        // Sprawdź, czy tabela 'BracketTypes' jest pusta
        if (!context.BracketTypes.Any())
        {
            context.BracketTypes.AddRange(
                new BracketType { Name = "Single Elimination" },
                new BracketType { Name = "Double Elimination" }
            );
            context.SaveChanges();
        }

        // Sprawdź, czy tabela 'Teams' jest pusta
        if (!context.Teams.Any())
        {
            var user = context.Users.First(); // Pobierz pierwszego użytkownika, żeby przypisać go jako twórcę
            context.Teams.Add(new Team
            {
                Name = "Team A",
                CreatorId = user.Id,
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();
        }

    }
}