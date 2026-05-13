namespace MedicalCertificate.Domain.Entities
{
    public class Edu_Positions
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public bool Deleted { get; set; }
        public string? Description { get; set; }
        public int Lectures { get; set; }
        public int Practices { get; set; }
        public int Labs { get; set; }
        public int? CategoryID { get; set; }
    }
}
