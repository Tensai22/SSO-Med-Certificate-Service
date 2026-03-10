namespace MedicalCertificate.Domain.Entities;

    public class RoleOperation
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        public int RoleId { get; set; }

        public Role? Role { get; set; }
        public Operation? Operation { get; set; }
    }