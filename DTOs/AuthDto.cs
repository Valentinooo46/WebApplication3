namespace WebApplication3.DTOs
{
    public record RegisterDto(string Email, string Password, string FirstName, string LastName, string IconUrl);
    public record LoginDto(string Email, string Password);

}
