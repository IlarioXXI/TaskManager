namespace TaskManagerWEB.Api.ViewModels.UserViewModels
{
    public class AppJWTSettings
    {
        public required string  Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
    }
}
