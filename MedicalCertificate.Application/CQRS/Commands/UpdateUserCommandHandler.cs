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
                UserName = request.UserName,
                RoleId = request.RoleId,
                RoleName = request.RoleName
            };

            var result = await _userService.UpdateAsync(request.Id, userDto);

            if (result.IsFailed)
                return Result.Failure<UserDto>(result.Error);

            return result;
        }
    }
}