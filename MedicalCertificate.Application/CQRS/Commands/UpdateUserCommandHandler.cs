using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            this._userService = userService;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = new UserDto
            {
                Id = request.Id,
                LastName = request.LastName,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                Email = request.Email,
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
            };

            var result = await _userService.UpdateAsync(request.Id, userDto);

            if (result.IsFailed)
                return Result.Failure<UserDto>(result.Error);

            return result;
        }
    }
}
