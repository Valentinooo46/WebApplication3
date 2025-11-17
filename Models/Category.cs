using System.Text.Json.Serialization;

namespace WebApplication3.Models
{
    public class Category
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("urlSlug")]
        public string UrlSlug { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

    }
}
