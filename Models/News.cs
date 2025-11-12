using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace WebApplication3.Models
{
    [Table("News")]
    public class New
    {
        [Key] public int ID{get;set; }
        [Required, StringLength(200)] public string title { get; set; } = null!;
        [Required, StringLength(500)] public string summary { get; set; } = null!;
        [Required, StringLength(500)] public string content { get; set; } = null!;
        [Required,StringLength(100)] public string slug { get; set; } = null!;
        [Required,StringLength(900)] public string image { get; set; } = null!;
    }
}
