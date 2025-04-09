namespace Turnament.Models;

public class TournamentTeam
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int TeamId { get; set; }

    public Tournament Tournament = new Tournament();
    public Team Team = new Team();
}