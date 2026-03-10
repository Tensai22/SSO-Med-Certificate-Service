namespace MedicalCertificate.Domain.Entities

{
    public class Operation
    {
        public int Id { get; set; }
        public string OperationName { get; set; }
        public string System {get; set;}
        public string CreatedBy {get; set;}
        public DateTime CreatedOn {get; set;}
        
        public ICollection<RoleOperation> RoleOperations { get; set; }
    }
}