namespace eClinicQueue.API.Models.Dtos.Auth;

public class DoctorRegisterDto
{
    public DoctorRegisterDto()
    {
        Email = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        PhoneNumber = string.Empty;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public List<string>? Specialization { get; set; }
}