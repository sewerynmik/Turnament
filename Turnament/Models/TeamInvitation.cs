namespace Turnament.Models;

public class TeamInvitation
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int InvitedUserId { get; set; }
    public int InvitedByUserId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Team Team { get; set; } = null!;
    public User InvitedUser { get; set; } = null!;
    public User InvitedByUser { get; set; } = null!;
}