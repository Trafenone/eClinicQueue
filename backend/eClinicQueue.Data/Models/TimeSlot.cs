namespace eClinicQueue.Data.Models;

public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
}
