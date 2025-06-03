using eClinicQueue.API.Models.Dtos.Auth;
using eClinicQueue.Data.Models;

namespace eClinicQueue.API.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> LoginByPhoneAsync(PhoneLoginDto loginDto);
    Task<AuthResponseDto> RegisterPatientAsync(PatientRegisterDto registerDto);
    Task<AuthResponseDto> RegisterDoctorAsync(DoctorRegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequest);
    string GenerateJwtToken(User user);
    Task<RefreshToken> GenerateRefreshTokenAsync(User user);
    Task<bool> RevokeTokenAsync(string token);
}
