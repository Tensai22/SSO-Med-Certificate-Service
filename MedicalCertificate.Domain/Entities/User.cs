namespace MedicalCertificate.Domain.Entities
{
    // TODO(copilot): transitional auth entity; remove when the schema is reduced to Edu_* + certificate models.
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string IIN { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int? EduUserId { get; set; }
        public Edu_Users? EduUser { get; set; }
    }
}
