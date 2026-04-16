using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Application.CQRS.Commands
{
    public class CreateUserCommand(
        string userName,
        string email,
        string password,
        int roleId,
        string iin) : IRequest<Result<UserDto>>
    {
        [Required]
        public string UserName { get;  } = userName;

        [Required]
        [EmailAddress]
        public string Email { get; } = email;

        [Required]
        [MinLength(6)]
        public string Password { get; } = password;

        public int RoleId { get; } = roleId;

        public string IIN { get; } = iin;
    }
}
