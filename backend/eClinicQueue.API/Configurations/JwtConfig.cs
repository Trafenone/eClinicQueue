namespace eClinicQueue.API.Configurations;

public class JwtConfig
{
    public const string SectionName = "JwtConfig";
    
    public JwtConfig()
    {
        Secret = string.Empty;
        Issuer = string.Empty;
        Audience = string.Empty;
    }

    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationMinutes { get; set; } 
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
