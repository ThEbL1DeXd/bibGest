using System.ComponentModel.DataAnnotations;

namespace bibGest.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = null!;

    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Le nom est requis")]
    [Display(Name = "Nom")]
    [StringLength(100)]
    public string Nom { get; set; } = null!;

    [Required(ErrorMessage = "Le prénom est requis")]
    [Display(Name = "Prénom")]
    [StringLength(100)]
    public string Prenom { get; set; } = null!;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmer le mot de passe")]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Téléphone")]
    [Phone(ErrorMessage = "Numéro de téléphone invalide")]
    public string? Telephone { get; set; }
}
