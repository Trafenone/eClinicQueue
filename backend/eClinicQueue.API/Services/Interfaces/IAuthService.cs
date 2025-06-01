using eClinicQueue.API.Dtos.Auth;
using eClinicQueue.Data.Models;

namespace eClinicQueue.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequest);
    string GenerateJwtToken(User user);
    Task<RefreshToken> GenerateRefreshTokenAsync(User user);
    Task<bool> RevokeTokenAsync(string token);
}
