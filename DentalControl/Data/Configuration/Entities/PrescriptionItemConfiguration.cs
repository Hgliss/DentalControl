using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de medicamentos de receta, sus columnas y relaciones.
/// </summary>
public class PrescriptionItemConfiguration
    : IEntityTypeConfiguration<PrescriptionItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("prescription_item");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.PrescriptionId)
            .HasColumnName("prescription_id")
            .IsRequired();

        builder.Property(x => x.MedicineName)
            .HasColumnName("medicine_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Dosage)
            .HasColumnName("dosage")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Frequency)
            .HasColumnName("frequency")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Duration)
            .HasColumnName("duration")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AdditionalInstructions)
            .HasColumnName("additional_instructions")
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

        builder.HasOne(x => x.Prescription)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PrescriptionId)
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