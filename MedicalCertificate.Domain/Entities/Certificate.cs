namespace MedicalCertificate.Domain.Entities
{
    public class Certificate
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // TODO(copilot): certificate stays, but the linked profile is still bridged through Edu_Users for now.
        public Edu_Users? User { get; set; }
        public CertificateStatus? Status { get; set; }
        public ICollection<CertificateStatusHistory>? StatusHistories { get; set; }= new List<CertificateStatusHistory>();
    }
}
