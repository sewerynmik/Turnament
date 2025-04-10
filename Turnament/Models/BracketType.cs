namespace Turnament.Models;

public class BracketType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}