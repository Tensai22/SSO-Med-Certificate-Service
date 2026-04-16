using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.WebAPI.Contracts;

public class CreateCertificateRequest
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(300)]
    public string Clinic { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int FilePathId { get; set; }

    [Range(1, 3)]
    public int StatusId { get; set; }

    [StringLength(1000)]
    public string ReviewerComment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
