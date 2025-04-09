namespace Turnament.Models;

public class Tournament
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int SportId { get; set; }
    public int BracketTypeId { get; set; }
    public int CreatorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int WinnerTeamId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Sport Sport = new Sport();
    public BracketType BracketType = new BracketType();
    public User Creator = new User();
    public Team WinnerTeam = new Team();

    public ICollection<TournamentTeam> TournamentTeams = new List<TournamentTeam>();
    public ICollection<Match> Matches = new List<Match>();
}