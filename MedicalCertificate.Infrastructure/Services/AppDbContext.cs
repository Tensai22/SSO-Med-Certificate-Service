using MedicalCertificate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedicalCertificate.Infrastructure.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // TODO(copilot): temporary auth table; remove when only Edu_* and certificate models remain.
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateStatusHistory> CertificateStatusHistories { get; set; }
        public DbSet<CertificateStatus> CertificateStatuses { get; set; }
        public DbSet<StoredFile> StoredFiles { get; set; }

        // TODO(copilot): transitional auth tables; remove once only Edu_* and certificate models remain.
        public DbSet<Edu_Users> Edu_Users { get; set; }
        public DbSet<Edu_Students> Edu_Students { get; set; }
        public DbSet<Edu_Employees> Edu_Employees { get; set; }
        public DbSet<Edu_OrgUnits> Edu_OrgUnits { get; set; }
        public DbSet<Edu_OrgUnitTypes> Edu_OrgUnitTypes { get; set; }
        public DbSet<Edu_EducationTypes> Edu_EducationTypes { get; set; }
        public DbSet<Edu_EmployeePositions> Edu_EmployeePositions { get; set; }
        public DbSet<Edu_Positions> Edu_Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            ApplyUtcConversion(modelBuilder, dateTimeConverter);

            modelBuilder.Entity<User>(entity =>
            {
                // TODO(copilot): this mapping is part of the auth bridge and should disappear in the Edu-only cleanup.
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.IIN).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.HasIndex(e => e.EduUserId).IsUnique();
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.EduUser)
                    .WithOne(e => e.User)
                    .HasForeignKey<User>(e => e.EduUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasData(
                    new Role { Id = 1, Name = "Office Registrar" },
                    new Role { Id = 2, Name = "Student" });
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("certificates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                entity.HasOne(c => c.Status)
                    .WithMany()
                    .HasForeignKey(c => c.StatusId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<CertificateStatusHistory>(entity =>
            {
                entity.ToTable("certificatestatushistories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Comment).IsRequired();
                entity.HasOne(e => e.Certificate)
                    .WithMany(c => c.StatusHistories)
                    .HasForeignKey(e => e.CertificateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                entity.HasOne(e => e.CertificateStatus)
                    .WithMany(s => s.StatusHistories)
                    .HasForeignKey(e => e.StatusId);
                entity.HasOne(e => e.ChangedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ChangedBy);
            });

            modelBuilder.Entity<CertificateStatus>(entity =>
            {
                entity.ToTable("certificatestatuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Status).IsRequired();
                entity.HasData(
                    new CertificateStatus { Id = 1, Status = "В обработке" },
                    new CertificateStatus { Id = 2, Status = "Принято" },
                    new CertificateStatus { Id = 3, Status = "Отклонено" });
            });

            modelBuilder.Entity<StoredFile>(entity =>
            {
                entity.ToTable("storedfiles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Bucket).IsRequired();
                entity.Property(e => e.ContentType).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.ObjectKey).IsRequired();
            });

            modelBuilder.Entity<Edu_Users>(entity =>
            {
                entity.ToTable("Edu_Users");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.LastUpdatedBy).IsRequired();
                entity.Property(e => e.LastUpdatedOn).IsRequired();
                entity.Property(e => e.Resident).IsRequired();
                entity.HasOne(e => e.Student)
                    .WithOne(s => s.User)
                    .HasForeignKey<Edu_Students>(s => s.StudentID);
                entity.HasOne(e => e.Employee)
                    .WithOne(e2 => e2.User)
                    .HasForeignKey<Edu_Employees>(e2 => e2.ID);
            });

            modelBuilder.Entity<Edu_Students>(entity =>
            {
                entity.ToTable("Edu_Students");
                entity.HasKey(e => e.StudentID);
                entity.Property(e => e.StudentID).ValueGeneratedNever();
                entity.Property(e => e.LastUpdatedBy).IsRequired();
                entity.Property(e => e.LastUpdatedOn).IsRequired();
                // TODO(copilot): this hierarchy join exists only for registrar/certificate profile resolution.
                entity.HasOne(e => e.Speciality)
                    .WithMany()
                    .HasForeignKey(e => e.SpecialityID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Edu_Employees>(entity =>
            {
                entity.ToTable("Edu_Employees");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Edu_OrgUnits>(entity =>
            {
                entity.ToTable("Edu_OrgUnits");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired();
                // TODO(copilot): keep the parent chain until institute/department lookups are no longer needed.
                entity.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentID)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Type)
                    .WithMany()
                    .HasForeignKey(e => e.TypeID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            modelBuilder.Entity<Edu_OrgUnitTypes>(entity =>
            {
                entity.ToTable("Edu_OrgUnitTypes");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Edu_EducationTypes>(entity =>
            {
                entity.ToTable("Edu_EducationTypes");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Edu_Positions>(entity =>
            {
                entity.ToTable("Edu_Positions");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Deleted).IsRequired();
            });

            modelBuilder.Entity<Edu_EmployeePositions>(entity =>
            {
                entity.ToTable("Edu_EmployeePositions");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.StartedOn).IsRequired();
                entity.Property(e => e.LastUpdatedBy).IsRequired();
                entity.Property(e => e.LastUpdatedOn).IsRequired();
                entity.HasOne(e => e.OrgUnit)
                    .WithMany()
                    .HasForeignKey(e => e.OrgUnitID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                entity.HasOne(e => e.Position)
                    .WithMany()
                    .HasForeignKey(e => e.PositionID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });
        }

        private static void ApplyUtcConversion(ModelBuilder modelBuilder, ValueConverter<DateTime, DateTime> converter)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }
}
