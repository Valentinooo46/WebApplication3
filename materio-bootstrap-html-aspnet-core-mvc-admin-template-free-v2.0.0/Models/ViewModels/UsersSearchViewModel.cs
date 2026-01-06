using System.Collections.Generic;

namespace AspnetCoreMvcFull.Models.ViewModels
{
 public class UsersSearchViewModel
 {
 public string Query { get; set; } = string.Empty;
 public List<UserListItemViewModel> Results { get; set; } = new List<UserListItemViewModel>();
 }
}
