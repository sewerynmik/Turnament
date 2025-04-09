namespace Turnament.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Creator { get; set; } = null!;

    public ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<Tournament> Tournaments = new List<Tournament>();
    public ICollection<TeamInvitation> Invitations { get; set; } = new List<TeamInvitation>();
    public ICollection<Match> MatchesAsTeam1 { get; set; } = new List<Match>();
    public ICollection<Match> MatchesAsTeam2 { get; set; } = new List<Match>();
    public ICollection<Match> MatchesWinners { get; set; } = new List<Match>();
}