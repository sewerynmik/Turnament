namespace Turnament.Models;

public class Match
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int Round { get; set; }
    public int Team1Id { get; set; }
    public int Team2Id { get; set; }
    public int WinnerId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime FinishedAt { get; set; }

    public Tournament Tournament = new Tournament();
    public Team Team1 = new Team();
    public Team Team2 = new Team();
    public Team Winner = new Team();
}