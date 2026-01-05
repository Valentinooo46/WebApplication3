using System.ComponentModel.DataAnnotations;

namespace AspnetCoreMvcFull.Models.ViewModels
{
  public class LoginViewModel
  {
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
  }
}
