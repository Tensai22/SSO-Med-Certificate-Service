using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Application.CQRS.Commands
{
public class CreateUserCommand(
        string lastName,
        string? firstName,
        string? middleName,
        string? email,
        string? personalEmail,
        DateOnly? dob,
        string? placeOfBirth,
        bool? male,
        string? homePhone,
        string? mobilePhone,
        string? iin,
        string? photoFileName,
        byte[]? photoFileData,
        Guid? fileContainerId,
        string? mobilePushId,
        int? oldId,
        int? esuvoId,
        Guid? extraFileContainerId,
        bool resident,
        int? heroPersonId,
        bool? isReadTeamsNotif,
        int? nationalityId,
        int? maritalStatusId,
        int? messengerTypeId,
        int? citizenshipCountryId,
        int? citizenCategoryId,
        string lastUpdatedBy) : IRequest<Result<UserDto>>
    {
        [Required]
        public string LastName { get; } = lastName;

        [EmailAddress]
        public string? Email { get; } = email;

        public string? FirstName { get; } = firstName;
        public string? MiddleName { get; } = middleName;
        public string? PersonalEmail { get; } = personalEmail;
        public DateOnly? DOB { get; } = dob;
        public string? PlaceOfBirth { get; } = placeOfBirth;
        public bool? Male { get; } = male;
        public string? HomePhone { get; } = homePhone;
        public string? MobilePhone { get; } = mobilePhone;
        public string? IIN { get; } = iin;
        public string? PhotoFileName { get; } = photoFileName;
        public byte[]? PhotoFileData { get; } = photoFileData;
        public Guid? FileContainerID { get; } = fileContainerId;
        public string? MobilePushID { get; } = mobilePushId;
        public int? oldId { get; } = oldId;
        public int? ESUVOID { get; } = esuvoId;
        public Guid? ExtraFileContainerID { get; } = extraFileContainerId;
        public bool Resident { get; } = resident;
        public int? Hero_Person_ID { get; } = heroPersonId;
        public bool? IsReadTeamsNotif { get; } = isReadTeamsNotif;
        public int? NationalityID { get; } = nationalityId;
        public int? MaritalStatusID { get; } = maritalStatusId;
        public int? MessengerTypeID { get; } = messengerTypeId;
        public int? CitizenshipCountryID { get; } = citizenshipCountryId;
        public int? CitizenCategoryID { get; } = citizenCategoryId;
        public string LastUpdatedBy { get; } = lastUpdatedBy;
    }
}
