namespace TaskManagerWEB.Api.models
{
    public class AppJWTSettings
    {
        public required string  Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
    }
}
