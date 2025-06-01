using eClinicQueue.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace eClinicQueue.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DoctorProfile>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.TimeSlot)
            .WithMany()
            .HasForeignKey(a => a.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeSlot>()
            .HasOne(t => t.Doctor)
            .WithMany(d => d.AvailableTimeSlots)
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Statistics>()
            .HasOne(s => s.Doctor)
            .WithMany()
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorProfile> DoctorProfiles { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Statistics> Statistics { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
