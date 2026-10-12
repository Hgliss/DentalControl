using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de cargos, sus columnas y relaciones.
/// </summary>
public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        builder.ToTable("charge");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.ConsultationId)
            .HasColumnName("consultation_id")
            .IsRequired(false);

        builder.Property(x => x.TreatmentId)
            .HasColumnName("treatment_id")
            .IsRequired(false);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.ChargeDate)
            .HasColumnName("charge_date")
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
            .WithMany(x => x.Charges)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Consultation)
            .WithMany(x => x.Charges)
            .HasForeignKey(x => x.ConsultationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Treatment)
            .WithMany(x => x.Charges)
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