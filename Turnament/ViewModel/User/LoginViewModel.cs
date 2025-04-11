using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.User;

public class LoginViewModel
{
    [Display(Name = "Email:")]
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format email.")]
    public string? Email { get; set; }
    
    [Display(Name = "Hasło:")]
    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    public string? Pass { get; set; }
}