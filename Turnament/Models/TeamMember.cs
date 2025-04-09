namespace Turnament.Models;

public class TeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Team Team = new Team();
    public User User = new User();
}