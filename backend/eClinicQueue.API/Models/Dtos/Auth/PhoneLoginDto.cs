namespace eClinicQueue.API.Models.Dtos.Auth;

public class PhoneLoginDto
{
    public PhoneLoginDto()
    {
        PhoneNumber = string.Empty;
        Password = string.Empty;
    }

    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}