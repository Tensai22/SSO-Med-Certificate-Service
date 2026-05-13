namespace MedicalCertificate.Domain.Entities
{
    // TODO(copilot): org-unit hierarchy is only needed to resolve institute/department during the transition.
    public class Edu_OrgUnits
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Title { get; set; } = null!;
        public bool Deleted { get; set; }
        public string? ShortTitle { get; set; }

        public int TypeID { get; set; }
        public Edu_OrgUnitTypes? Type { get; set; }
        public Edu_OrgUnits? Parent { get; set; }
        public ICollection<Edu_OrgUnits> Children { get; set; } = new List<Edu_OrgUnits>();
    }
}
