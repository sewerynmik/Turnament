namespace Turnament.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Creator = new User();

    public ICollection<TournamentTeam> TournamentTeams = new List<TournamentTeam>();
    public ICollection<TeamMember> TeamMembers = new List<TeamMember>();
    public ICollection<Tournament> Tournaments = new List<Tournament>();
    public ICollection<TeamInvitation> TeamInvitations = new List<TeamInvitation>();
    public ICollection<User> Users = new List<User>();
    public ICollection<Match> MatchesAsTeam1 = new List<Match>();
    public ICollection<Match> MatchesAsTeam2 = new List<Match>();
    public ICollection<Match> MatchesWinners = new List<Match>();
}