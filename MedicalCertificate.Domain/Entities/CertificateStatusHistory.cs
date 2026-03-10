namespace MedicalCertificate.Domain.Entities
{
    public class CertificateStatusHistory
    {
        public int Id { get; set; }
        public int CertificateId { get; set; }
        public DateTime ChangedAt { get; set; }
        public int StatusId { get; set; }
        public int ChangedBy { get; set; }
        public string Comment { get; set; } = string.Empty;

        public Certificate? Certificate { get; set; }
        public CertificateStatus? CertificateStatus { get; set; }
        public User? ChangedByUser { get; set; }
    }
}