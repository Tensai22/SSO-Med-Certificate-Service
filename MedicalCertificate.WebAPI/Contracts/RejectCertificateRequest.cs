using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.WebAPI.Contracts;

public class RejectCertificateRequest
{
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;
}
