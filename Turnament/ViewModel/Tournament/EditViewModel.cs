using System.ComponentModel.DataAnnotations;

namespace Turnament.ViewModel.Tournament;

public class EditViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Nazwa")]
    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa misie mieć minimum 3 znaki.")]
    public required string Name { get; set; }
    
    [Display(Name = "Opis")]
    public string? Description { get; set; }
    
    [Display(Name = "Sport")]
    public int SportId { get; set; }
    
    [Display(Name = "Rodzaj drabinki")]
    public int BracketTypeId { get; set; }
    
    [Display(Name = "Dara rozpoczęcia")]
    [Required(ErrorMessage = "Wprowadz poprawną datę.")]
    [DataType(DataType.DateTime)]
    public DateTime? StartDate { get; set; }
    
    [Display(Name = "Dara zakonczenia.")]
    [Required(ErrorMessage = "Wprowadz poprawną datę.")]
    [DataType(DataType.DateTime)]
    public DateTime? EndDate { get; set; }
    
    [Display(Name = "Zwycięsca")]
    public int? WinnerTeamId { get; set; }
}