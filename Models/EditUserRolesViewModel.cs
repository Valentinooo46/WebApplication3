using WebApplication3.Models;

public class EditUserRolesViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<RoleSelection> Roles { get; set; }
}