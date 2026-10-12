using DentalControl.Models.Catalog;
using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalControl.Data.Context;

/// <summary>
/// Representa el contexto de base de datos de DentalControl.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientHistory> PatientHistories => Set<PatientHistory>();
    public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();
    public DbSet<PatientMedication> PatientMedications => Set<PatientMedication>();
    public DbSet<PatientCondition> PatientConditions => Set<PatientCondition>();

    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();

    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    public DbSet<Charge> Charges => Set<Charge>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentDetail> PaymentDetails => Set<PaymentDetail>();

    public DbSet<MedicalCertificate> MedicalCertificates
        => Set<MedicalCertificate>();

    public DbSet<CertificateTemplate> CertificateTemplates
        => Set<CertificateTemplate>();

    public DbSet<CatalogCategory> CatalogCategories => Set<CatalogCategory>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}