namespace WebApplication3.Services
{
    public record GoogleSettings
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string RedirectUri { get; init; }
    }
}
