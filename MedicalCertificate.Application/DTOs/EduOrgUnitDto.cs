namespace MedicalCertificate.Application.DTOs
{
    public class EduOrgUnitDto
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Title { get; set; } = null!;
        public bool Deleted { get; set; }
        public string? ShortTitle { get; set; }
        public int TypeID { get; set; }
    }
}
