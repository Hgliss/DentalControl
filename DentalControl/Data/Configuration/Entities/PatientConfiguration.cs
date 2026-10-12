using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Entities;

/// <summary>
/// Configura la tabla de pacientes, sus columnas y relaciones.
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("patient_id");

        builder.Property(x => x.PatientCode)
            .HasColumnName("patient_code")
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.PatientCode)
            .IsUnique();

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MiddleName)
            .HasColumnName("middle_name")
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SecondLastName)
            .HasColumnName("second_last_name")
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        builder.Property(x => x.GenderId)
            .HasColumnName("gender_id")
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .HasColumnName("address")
            .HasMaxLength(500);

        builder.Property(x => x.Cui)
            .HasColumnName("cui")
            .HasMaxLength(13);

        builder.HasIndex(x => x.Cui)
            .IsUnique();

        builder.Property(x => x.Occupation)
            .HasColumnName("occupation")
            .HasMaxLength(100);

        builder.Property(x => x.ReferredBy)
            .HasColumnName("referred_by")
            .HasMaxLength(100);

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

        builder.HasOne(x => x.Gender)
            .WithMany()
            .HasForeignKey(x => x.GenderId)
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