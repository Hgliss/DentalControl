using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de medicamentos del paciente, sus columnas y relaciones.
/// </summary>
public class PatientMedicationConfiguration
    : IEntityTypeConfiguration<PatientMedication>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PatientMedication> builder)
    {
        builder.ToTable("patient_medication");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.MedicineId)
            .HasColumnName("medicine_id")
            .IsRequired(false);

        builder.Property(x => x.OtherMedicineName)
            .HasColumnName("other_medicine_name")
            .IsRequired(false)
            .HasMaxLength(150);

        builder.Property(x => x.Dosage)
            .HasColumnName("dosage")
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.Frequency)
            .HasColumnName("frequency")
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.AdministrationRoute)
            .HasColumnName("administration_route")
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired(false);

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired(false);

        builder.Property(x => x.IsCurrent)
            .HasColumnName("is_current")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .IsRequired(false);

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
            .WithMany(x => x.Medications)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Medicine)
            .WithMany()
            .HasForeignKey(x => x.MedicineId)
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