namespace eClinicQueue.Data.Models
{
    public class Patient
    {
        public Patient()
        {
            Appointments = new List<Appointment>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MedicalRecordNumber { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
