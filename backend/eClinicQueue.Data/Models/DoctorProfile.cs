namespace eClinicQueue.Data.Models
{
    public class DoctorProfile
    {
        public DoctorProfile()
        {
            Specialization = new List<string>();
            Appointments = new List<Appointment>();
            AvailableTimeSlots = new List<TimeSlot>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<string> Specialization { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<TimeSlot> AvailableTimeSlots { get; set; }
    }
}
