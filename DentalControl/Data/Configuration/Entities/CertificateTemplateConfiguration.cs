using DentalControl.Models.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalControl.Data.Configuration.Catalog;

/// <summary>
/// Configura la tabla de plantillas de constancias y sus columnas.
/// </summary>
public class CertificateTemplateConfiguration
    : IEntityTypeConfiguration<CertificateTemplate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CertificateTemplate> builder)
    {
        builder.ToTable("certificate_template");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);
    }
}