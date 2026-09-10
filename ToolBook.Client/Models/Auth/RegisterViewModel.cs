using System.ComponentModel.DataAnnotations;

namespace ToolBook.Client.Models.Auth;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required] 
    [Compare(nameof(Password), ErrorMessage = "Adgangskoderne matcher ikke")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}