namespace eClinicQueue.Data.Models
{
    public class Statistics
    {
        public Guid Id { get; set; }
        public Guid? DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; }
        public DateTime Date { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CanceledAppointments { get; set; }
        public double AverageAppointmentDuration { get; set; }
    }
}
