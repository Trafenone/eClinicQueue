using eClinicQueue.Data.Models.Enums;

namespace eClinicQueue.Data.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorProfile Doctor { get; set; }
    public Guid TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; }
}
