using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de consultas, sus columnas y relaciones.
/// </summary>
public class ConsultationConfiguration
    : IEntityTypeConfiguration<Consultation>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("consultation");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.TreatmentId)
            .HasColumnName("treatment_id")
            .IsRequired(false);

        builder.Property(x => x.ConsultationDate)
            .HasColumnName("consultation_date")
            .IsRequired();

        builder.Property(x => x.ConsultationType)
            .HasColumnName("consultation_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReasonForVisit)
            .HasColumnName("reason_for_visit")
            .IsRequired();

        builder.Property(x => x.SystolicPressure)
            .HasColumnName("systolic_pressure")
            .IsRequired(false);

        builder.Property(x => x.DiastolicPressure)
            .HasColumnName("diastolic_pressure")
            .IsRequired(false);

        builder.Property(x => x.HeartRate)
            .HasColumnName("heart_rate")
            .IsRequired(false);

        builder.Property(x => x.OxygenSaturation)
            .HasColumnName("oxygen_saturation")
            .HasPrecision(5, 2)
            .IsRequired(false);

        builder.Property(x => x.Temperature)
            .HasColumnName("temperature")
            .HasPrecision(4, 2)
            .IsRequired(false);

        builder.Property(x => x.PhysicalNotes)
            .HasColumnName("physical_notes")
            .IsRequired(false);

        builder.Property(x => x.DiagnosisDescription)
            .HasColumnName("diagnosis_description")
            .IsRequired(false);

        builder.Property(x => x.ProcedureDescription)
            .HasColumnName("procedure_description")
            .IsRequired(false);

        builder.Property(x => x.MedicalObservations)
            .HasColumnName("medical_observations")
            .IsRequired(false);

        builder.Property(x => x.RequiresFollowUp)
            .HasColumnName("requires_follow_up")
            .IsRequired();

        builder.Property(x => x.NextAppointment)
            .HasColumnName("next_appointment")
            .IsRequired(false);

        builder.Property(x => x.ConsultationCost)
            .HasColumnName("consultation_cost")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.Property(x => x.DeletedBy)
            .HasColumnName("deleted_by");

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Consultations)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Treatment)
            .WithMany(x => x.Consultations)
            .HasForeignKey(x => x.TreatmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}