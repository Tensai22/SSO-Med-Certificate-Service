using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedicalCertificate.Application.CQRS.Commands;

public class CreateUserCommandHandler(IUserService userService,ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Отправил запрос на создание записи пользователя");
        var result = await userService.CreateAsync(new UserDto
        {
            LastName = request.LastName,
            Email = request.Email,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            PersonalEmail = request.PersonalEmail,
            DOB = request.DOB,
            PlaceOfBirth = request.PlaceOfBirth,
            Male = request.Male,
            HomePhone = request.HomePhone,
            MobilePhone = request.MobilePhone,
            IIN = request.IIN,
            PhotoFileName = request.PhotoFileName,
            PhotoFileData = request.PhotoFileData,
            FileContainerID = request.FileContainerID,
            MobilePushID = request.MobilePushID,
            oldId = request.oldId,
            ESUVOID = request.ESUVOID,
            ExtraFileContainerID = request.ExtraFileContainerID,
            Resident = request.Resident,
            Hero_Person_ID = request.Hero_Person_ID,
            IsReadTeamsNotif = request.IsReadTeamsNotif,
            NationalityID = request.NationalityID,
            MaritalStatusID = request.MaritalStatusID,
            MessengerTypeID = request.MessengerTypeID,
            CitizenshipCountryID = request.CitizenshipCountryID,
            CitizenCategoryID = request.CitizenCategoryID,
            LastUpdatedBy = request.LastUpdatedBy,
            LastUpdatedOn = DateTime.UtcNow
        }, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogWarning("Возникла ошибка");
            return Result.Failure<UserDto>(result.Error);
        }

        logger.LogInformation("Пользователь создан");
        return result;
    }
}
