using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de padecimientos del paciente, sus columnas y relaciones.
/// </summary>
public class PatientConditionConfiguration
    : IEntityTypeConfiguration<PatientCondition>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PatientCondition> builder)
    {
        builder.ToTable("patient_condition");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.ConditionId)
            .HasColumnName("condition_id")
            .IsRequired(false);

        builder.Property(x => x.OtherConditionName)
            .HasColumnName("other_condition_name")
            .IsRequired(false)
            .HasMaxLength(150);

        builder.Property(x => x.DiagnosisDate)
            .HasColumnName("diagnosis_date")
            .IsRequired(false);

        builder.Property(x => x.ClinicalStatusId)
            .HasColumnName("clinical_status_id")
            .IsRequired(false);

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
            .WithMany(x => x.Conditions)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Condition)
            .WithMany()
            .HasForeignKey(x => x.ConditionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClinicalStatus)
            .WithMany()
            .HasForeignKey(x => x.ClinicalStatusId)
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