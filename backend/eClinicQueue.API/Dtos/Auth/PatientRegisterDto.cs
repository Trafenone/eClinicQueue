namespace eClinicQueue.API.Dtos.Auth;

public class PatientRegisterDto
{
    public PatientRegisterDto()
    {
        PhoneNumber = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? MedicalRecordNumber { get; set; }
}