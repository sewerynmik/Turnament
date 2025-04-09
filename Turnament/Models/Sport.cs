namespace Turnament.Models;

public class Sport
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Tournament> Tournaments = new List<Tournament>();
}