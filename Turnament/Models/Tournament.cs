namespace Turnament.Models;

public class Tournament
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SportId { get; set; }
    public int BracketTypeId { get; set; }
    public int CreatorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? WinnerTeamId { get; set; }

    public Sport Sport { get; set; } = null!;
    public BracketType BracketType { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public Team WinnerTeam { get; set; } = null!;

    public ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}