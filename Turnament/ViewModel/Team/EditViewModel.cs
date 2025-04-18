using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.Team;

public class EditViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Nazwa")]
    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    public required string Name { get; set; }
}