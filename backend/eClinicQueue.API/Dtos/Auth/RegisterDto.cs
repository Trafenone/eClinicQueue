namespace eClinicQueue.API.Dtos.Auth;

public class RegisterDto
{
    public RegisterDto()
    {
        Email = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Role = string.Empty;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
