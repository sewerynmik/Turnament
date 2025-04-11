using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.User;

public class LoginViewModel
{
    [Display(Name = "Email:")]
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format email.")]
    public required string Email { get; set; }
    
    [Display(Name = "Hasło:")]
    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    public required string Pass { get; set; }
}