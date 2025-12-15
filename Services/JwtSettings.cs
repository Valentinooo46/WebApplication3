namespace WebApplication3.Services
{
    public record JwtSettings
    {
        public string Key { get; init; }    // symmetric key for signing app tokens
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpMinutes { get; init; }
    }
}
