namespace MedicalCertificate.Domain.Entities
{
    public class Edu_Employees
    {
        public int ID { get; set; }
        public bool IsAdvisor { get; set; }
        public int? RoleGroupId { get; set; }

        public Edu_Users? User { get; set; }
    }
}
