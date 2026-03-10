using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Queries
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDto?>>
    {
        private readonly IUserService _userService;

        public GetUserByEmailQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UserDto?>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetByEmailAsync(request.Email);
        }
    }
}