using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.User;

public class RegisterViewModel
{
    [Display(Name = "Nazwa użytkownika:")]
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public required string Username { set; get; }
    
    [Display(Name = "Email:")]
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public required string Email { get; set; }
    
    [Display(Name = "Hasło:")]
    [Required(ErrorMessage = "Hasło jest wymagane")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmiej 6 znaków.")]
    public required string Pass { get; set; }

    [Display(Name = "Potwierdź hasło:")]
    [Required(ErrorMessage = "Hasło jest wymagane")]
    [DataType(DataType.Password)]
    [Compare("Pass", ErrorMessage = "Hasła nie są takie same")]
    public required string ConfirmPass { get; set; }
}