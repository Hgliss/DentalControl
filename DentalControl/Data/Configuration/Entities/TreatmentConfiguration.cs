using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de tratamientos, sus columnas y relaciones.
/// </summary>
public class TreatmentConfiguration
    : IEntityTypeConfiguration<Treatment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Treatment> builder)
    {
        builder.ToTable("treatment");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.TreatmentType)
            .HasColumnName("treatment_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(x => x.TotalCost)
            .HasColumnName("total_cost")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.EstimatedSessionCount)
            .HasColumnName("estimated_session_count")
            .IsRequired(false);

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired(false);

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired(false);

        builder.Property(x => x.TreatmentStatusId)
            .HasColumnName("treatment_status_id")
            .IsRequired();

        builder.Property(x => x.ProgressNotes)
            .HasColumnName("progress_notes")
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
            .WithMany(x => x.Treatments)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TreatmentStatus)
            .WithMany()
            .HasForeignKey(x => x.TreatmentStatusId)
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