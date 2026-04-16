using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.WebAPI.Contracts;

public class CreateUserRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }

    [StringLength(12, MinimumLength = 12)]
    public string IIN { get; set; } = string.Empty;
}
