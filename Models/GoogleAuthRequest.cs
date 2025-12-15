namespace WebApplication3.Models
{
    public class GoogleAuthRequest
    {
        public string Code { get; set; }
        // optional: якщо frontend використовує різні redirect URIs
        public string CodeVerifier { get; set; }
        public string RedirectUri { get; set; }
    }
}
