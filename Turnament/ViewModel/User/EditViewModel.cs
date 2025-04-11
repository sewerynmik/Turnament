using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.User;

public class EditViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Nazwa użytkownika")]
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public required string Username { get; set; }
    
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
    public required string Email { get; set; }
    
    [Display(Name = "Nowe hasło")]
    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
    public string? Pass { get; set; }
    
    [Display(Name = "Potwierdz nowe hasło")]
    [Required(ErrorMessage = "Hasło jest wymagane")]
    [DataType(DataType.Password)]
    [Compare("Pass", ErrorMessage = "Hasła nie są takie same.")]
    public string? PassConfirm { get; set; }
}