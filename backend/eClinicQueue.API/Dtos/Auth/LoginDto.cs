namespace eClinicQueue.API.Dtos.Auth;

public class LoginDto
{
    public LoginDto()
    {
        Email = string.Empty;
        Password = string.Empty;
    }

    public string Email { get; set; }
    public string Password { get; set; }
}
