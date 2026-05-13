namespace MedicalCertificate.Domain.Entities
{
    public class Edu_EmployeePositions
    {
        public int ID { get; set; }
        public DateOnly StartedOn { get; set; }
        public DateOnly? EndedOn { get; set; }
        public string LastUpdatedBy { get; set; } = null!;
        public DateTime LastUpdatedOn { get; set; }
        public double? Rate { get; set; }
        public bool? IsMainPosition { get; set; }
        public int? HrOrderId { get; set; }

        public int OrgUnitID { get; set; }
        public int PositionID { get; set; }
        public int EmployeeID { get; set; }

        public Edu_OrgUnits? OrgUnit { get; set; }
        public Edu_Positions? Position { get; set; }
        public Edu_Employees? Employee { get; set; }
    }
}
