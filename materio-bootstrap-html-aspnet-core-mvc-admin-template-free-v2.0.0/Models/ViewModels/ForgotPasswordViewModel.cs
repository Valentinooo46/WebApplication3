using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models.ViewModels
{
 public class ForgotPasswordViewModel
 {
 [Required(ErrorMessage = "Email є обов'язковим")]
 [EmailAddress(ErrorMessage = "Невірний формат email")]
 public string Email { get; set; } = string.Empty;
 }
}
