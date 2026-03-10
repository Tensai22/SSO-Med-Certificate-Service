using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Text.Json.Serialization;

namespace MedicalCertificate.Application.CQRS.Commands
{
    public class CreateUserCommand(string userName, int roleId) : IRequest<Result<UserDto>>
    {
        public string UserName { get;  } = userName;

        public int RoleId { get; } = roleId;
    }
}