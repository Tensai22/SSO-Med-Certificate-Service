using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Domain.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; }
    }
}