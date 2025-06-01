namespace eClinicQueue.API.Dtos.Auth;

public class TokenRequestDto
{
    public TokenRequestDto()
    {
        Token = string.Empty;
        RefreshToken = string.Empty;
    }
    
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
