using eClinicQueue.API.Core.Interfaces;
using eClinicQueue.API.Models.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eClinicQueue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new patient
    /// </summary>
    [HttpPost("register/patient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RegisterPatient([FromBody] PatientRegisterDto registerDto)
    {
        _logger.LogInformation("Processing patient registration request");
        var result = await _authService.RegisterPatientAsync(registerDto);
        _logger.LogInformation("Patient registered successfully");
        return Ok(result);
    }

    /// <summary>
    /// Register a new doctor (admin only)
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [HttpPost("register/doctor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RegisterDoctor([FromBody] DoctorRegisterDto registerDto)
    {
        _logger.LogInformation("Processing doctor registration request");
        var result = await _authService.RegisterDoctorAsync(registerDto);
        _logger.LogInformation("Doctor registered successfully");
        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Processing login request for {Email}", loginDto.Email);
        var result = await _authService.LoginAsync(loginDto);
        _logger.LogInformation("Login successful for {Email}", loginDto.Email);
        return Ok(result);
    }

    /// <summary>
    /// Login with phone number and password
    /// </summary>
    [HttpPost("login/phone")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> LoginByPhone([FromBody] PhoneLoginDto loginDto)
    {
        _logger.LogInformation("Processing phone login request for {PhoneNumber}", loginDto.PhoneNumber);
        var result = await _authService.LoginByPhoneAsync(loginDto);
        _logger.LogInformation("Phone login successful for {PhoneNumber}", loginDto.PhoneNumber);
        return Ok(result);
    }

    /// <summary>
    /// Refresh an authentication token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] TokenRequestDto tokenRequest)
    {
        _logger.LogInformation("Processing token refresh request");
        var result = await _authService.RefreshTokenAsync(tokenRequest);
        _logger.LogInformation("Token refresh successful");
        return Ok(result);
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    [Authorize]
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeToken([FromBody] TokenRequestDto tokenRequest)
    {
        _logger.LogInformation("Processing token revocation request");
        var success = await _authService.RevokeTokenAsync(tokenRequest.RefreshToken);

        if (!success)
        {
            _logger.LogWarning("Token not found for revocation");
            return NotFound(new { message = "Token not found" });
        }

        _logger.LogInformation("Token revoked successfully");
        return Ok(new { message = "Token revoked successfully" });
    }
}
