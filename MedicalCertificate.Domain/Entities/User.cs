namespace MedicalCertificate.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string IIN { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
    }
}