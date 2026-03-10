namespace MedicalCertificate.Domain.Entities;
public class StoredFile
    {
        public int Id { get; set; }
        public string Bucket { get; set; }
        public string ObjectKey { get; set; }
        public string Name { get; set; }
        public string ContentType  { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Certificate>? Certificates { get; set; }
    }