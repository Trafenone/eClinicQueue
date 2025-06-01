using eClinicQueue.API.Dtos.Auth;
using eClinicQueue.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace eClinicQueue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register/patient")]
    public async Task<ActionResult<AuthResponseDto>> RegisterPatient([FromBody] PatientRegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterPatientAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during patient registration" });
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("register/doctor")]
    public async Task<ActionResult<AuthResponseDto>> RegisterDoctor([FromBody] DoctorRegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterDoctorAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during doctor registration" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("login/phone")]
    public async Task<ActionResult<AuthResponseDto>> LoginByPhone([FromBody] PhoneLoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginByPhoneAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] TokenRequestDto tokenRequest)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(tokenRequest);
            return Ok(result);
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while refreshing the token" });
        }
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] TokenRequestDto tokenRequest)
    {
        try
        {
            var success = await _authService.RevokeTokenAsync(tokenRequest.RefreshToken);
            
            if (!success)
            {
                return NotFound(new { message = "Token not found" });
            }

            return Ok(new { message = "Token revoked successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while revoking the token" });
        }
    }
}
