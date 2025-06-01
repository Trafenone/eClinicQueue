using eClinicQueue.API.Configurations;
using eClinicQueue.API.Dtos.Auth;
using eClinicQueue.API.Services.Interfaces;
using eClinicQueue.Data;
using eClinicQueue.Data.Models;
using eClinicQueue.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eClinicQueue.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtConfig _jwtConfig;

    public AuthService(ApplicationDbContext context, IOptions<JwtConfig> jwtOptions)
    {
        _context = context;
        _jwtConfig = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        var token = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            UserRole = user.Role.ToString(),
            UserId = user.Id.ToString()
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber ?? string.Empty,
            Role = Enum.TryParse<UserRole>(registerDto.Role, true, out var role) ? role : UserRole.Patient,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _context.Users.AddAsync(user);

        if (registerDto.Role == "Patient" && registerDto.DateOfBirth.HasValue)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DateOfBirth = registerDto.DateOfBirth.Value
            };
            await _context.Patients.AddAsync(patient);
        }
        else if (registerDto.Role == "Doctor")
        {
            var doctor = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _context.DoctorProfiles.AddAsync(doctor);
        }

        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            UserRole = user.Role.ToString(),
            UserId = user.Id.ToString()
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequest)
    {
        var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);

        if (principal == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var user = await _context.Users.FindAsync(Guid.Parse(userId));

        if (user == null || !user.IsActive)
        {
            throw new SecurityTokenException("User not found or inactive");
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == tokenRequest.RefreshToken && r.UserId == user.Id);

        if (refreshToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Refresh token expired");
        }

        if (refreshToken.IsUsed || refreshToken.IsRevoked)
        {
            throw new SecurityTokenException("Refresh token has been used or revoked");
        }

        refreshToken.IsUsed = true;
        _context.RefreshTokens.Update(refreshToken);

        var newJwtToken = GenerateJwtToken(user);
        var newRefreshToken = await GenerateRefreshTokenAsync(user);

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken.Token,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            UserRole = user.Role.ToString(),
            UserId = user.Id.ToString()
        };
    }

    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            SigningCredentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(User user)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(randomBytes),
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            IsUsed = false,
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);

        if (refreshToken == null)
        {
            return false;
        }

        refreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();

        return true;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
