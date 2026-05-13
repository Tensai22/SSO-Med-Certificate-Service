namespace MedicalCertificate.Domain.Entities
{
    public class Edu_Students
    {
        public int StudentID { get; set; }
        public int? SpecialityID { get; set; }
        public int? StatusID { get; set; }
        public int? CategoryID { get; set; }
        public bool NeedsDorm { get; set; }
        public bool AltynBelgi { get; set; }
        public int Year { get; set; }
        public int? RupID { get; set; }
        public DateOnly? EntryDate { get; set; }
        public double? GPA { get; set; }
        public string LastUpdatedBy { get; set; } = null!;
        public DateTime LastUpdatedOn { get; set; }
        public DateTime? GraduatedOn { get; set; }
        public DateOnly? AcademicStatusEndsOn { get; set; }
        public DateOnly? AcademicStatusStartsOn { get; set; }
        public double? GPA_Y { get; set; }
        public bool? IsPersonalDataComplete { get; set; }
        public int? HosterPrivelegeID { get; set; }
        public int? MinorSpecialityID { get; set; }
        public int? EnrollmentTypeId { get; set; }
        public double? EctsGPA { get; set; }
        public double? EctsGPA_Y { get; set; }
        public bool? IsScholarship { get; set; }
        public int? ScholarshipTypeID { get; set; }
        public string? ScholarshipOrderNumber { get; set; }
        public DateOnly? ScholarshipOrderDate { get; set; }
        public DateOnly? ScholarshipDateStart { get; set; }
        public DateOnly? ScholarshipDateEnd { get; set; }
        public int? FundingID { get; set; }
        public bool? IsKNB { get; set; }
        public int? EducationTypeID { get; set; }
        public int? EducationPaymentTypeID { get; set; }
        public int? GrantTypeID { get; set; }
        public int? EducationDurationID { get; set; }
        public int? StudyLanguageID { get; set; }
        public int? AcademicStatusID { get; set; }
        public int? AdvisorID { get; set; }

        // TODO(copilot): keep this relation until the certificate/registrar flow no longer needs department/institute lookup.
        public Edu_OrgUnits? Speciality { get; set; }
        public Edu_Users? User { get; set; }
    }
}
