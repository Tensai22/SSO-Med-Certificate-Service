using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Commands
{
    public record UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public int Id { get; init; }
        public string UserName { get; init; } = null!;
        public int RoleId { get; init; }
        public string RoleName { get; init; } = null!;
    }
}