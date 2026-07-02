using DentalControl.Models.Catalog;
using DentalControl.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalControl.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();

    public DbSet<Charge> Charges => Set<Charge>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentDetail> PaymentDetails => Set<PaymentDetail>();

    public DbSet<MedicalCertificate> MedicalCertificates => Set<MedicalCertificate>();

    public DbSet<ConsultationType> ConsultationTypes => Set<ConsultationType>();
    public DbSet<TreatmentType> TreatmentTypes => Set<TreatmentType>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<CertificateTemplate> CertificateTemplates => Set<CertificateTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}