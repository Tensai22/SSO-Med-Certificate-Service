namespace MedicalCertificate.Application.DTOs;

    public class CertificateDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Clinic { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int FilePathId { get; set; }
        public int StatusId { get; set; }
        public string ReviewerComment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
