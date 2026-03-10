using MedicalCertificate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedicalCertificate.Infrastructure.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateStatusHistory> CertificateStatusHistories { get; set; }
        public DbSet<CertificateStatus> CertificateStatuses { get; set; }
        public DbSet<StoredFile> StoredFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Авто-конвертация всех DateTime в UTC для PostgreSQL
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                v => v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }

                // Приведение имен таблиц к нижнему регистру
                entityType.SetTableName(entityType.GetTableName().ToLower());
            }

            // 2. Настройка связей
            modelBuilder.Entity<User>()
                .HasQueryFilter(e => !e.IsDeleted)
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Office Registrar" },
                new Role { Id = 2, Name = "Student" });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Status).WithMany().HasForeignKey(c => c.StatusId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}