namespace MedicalCertificate.Domain.Entities
{
    // TODO(copilot): keep this as the source of truth for profile data and prune any leftover auth-only links later.
    public class Edu_Users
    {
        public int ID { get; set; }
        public string LastName { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? PersonalEmail { get; set; }
        public DateOnly? DOB { get; set; }
        public string? PlaceOfBirth { get; set; }
        public bool? Male { get; set; }
        public string? HomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? IIN { get; set; }
        public string? PhotoFileName { get; set; }
        public byte[]? PhotoFileData { get; set; }
        public string LastUpdatedBy { get; set; } = null!;
        public DateTime LastUpdatedOn { get; set; }
        public Guid? FileContainerID { get; set; }
        public string? MobilePushID { get; set; }
        public int? oldId { get; set; }
        public int? ESUVOID { get; set; }
        public Guid? ExtraFileContainerID { get; set; }
        public bool Resident { get; set; }
        public int? Hero_Person_ID { get; set; }
        public bool? IsReadTeamsNotif { get; set; }
        public int? NationalityID { get; set; }
        public int? MaritalStatusID { get; set; }
        public int? MessengerTypeID { get; set; }
        public int? CitizenshipCountryID { get; set; }
        public int? CitizenCategoryID { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
        public string ShortName => $"{LastName} {FirstName?.FirstOrDefault()}. {MiddleName?.FirstOrDefault()}.".Trim();

        public User? User { get; set; }
        public Edu_Students? Student { get; set; }
        public Edu_Employees? Employee { get; set; }
    }
}
