using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.Team;

public class CreateViewModel
{
    [Required(ErrorMessage = "Nazwa jest wymagana")]
    [Display(Name = "Nazwa")]
    public string? Name { get; set; }
}