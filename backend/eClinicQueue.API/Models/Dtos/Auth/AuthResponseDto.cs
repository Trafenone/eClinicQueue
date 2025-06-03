namespace eClinicQueue.API.Models.Dtos.Auth;

public class AuthResponseDto
{
    public AuthResponseDto()
    {
        Token = string.Empty;
        RefreshToken = string.Empty;
        UserRole = string.Empty;
        UserId = string.Empty;
    }

    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
    public string UserRole { get; set; }
    public string UserId { get; set; }
}
