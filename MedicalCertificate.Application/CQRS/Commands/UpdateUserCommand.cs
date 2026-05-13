using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Commands
{
public record UpdateUserCommand : IRequest<Result<UserDto>>
{
    public int Id { get; init; }
    public string LastName { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? Email { get; init; }
    public string? PersonalEmail { get; init; }
    public DateOnly? DOB { get; init; }
    public string? PlaceOfBirth { get; init; }
    public bool? Male { get; init; }
    public string? HomePhone { get; init; }
    public string? MobilePhone { get; init; }
    public string? IIN { get; init; }
    public string? PhotoFileName { get; init; }
    public byte[]? PhotoFileData { get; init; }
    public Guid? FileContainerID { get; init; }
    public string? MobilePushID { get; init; }
    public int? oldId { get; init; }
    public int? ESUVOID { get; init; }
    public Guid? ExtraFileContainerID { get; init; }
    public bool Resident { get; init; }
    public int? Hero_Person_ID { get; init; }
    public bool? IsReadTeamsNotif { get; init; }
    public int? NationalityID { get; init; }
    public int? MaritalStatusID { get; init; }
    public int? MessengerTypeID { get; init; }
    public int? CitizenshipCountryID { get; init; }
    public int? CitizenCategoryID { get; init; }
    public string LastUpdatedBy { get; init; } = null!;
}
}
