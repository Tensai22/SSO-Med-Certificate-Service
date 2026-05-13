using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.WebAPI.Contracts;

public class CreateUserRequest
{
    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [EmailAddress]
    public string? PersonalEmail { get; set; }

    public DateOnly? DOB { get; set; }

    public string? PlaceOfBirth { get; set; }

    public bool? Male { get; set; }

    public string? HomePhone { get; set; }

    public string? MobilePhone { get; set; }

    [StringLength(12, MinimumLength = 12)]
    public string? IIN { get; set; }

    public string? PhotoFileName { get; set; }

    public byte[]? PhotoFileData { get; set; }

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
}
