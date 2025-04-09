namespace Turnament.Models;

public class TeamInvitation
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int InvitedUserId { get; set; }
    public int InvitedByUserId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Team Team = new Team();
    public User InvitedUser = new User();
    public User InvitedByUser = new User();
}