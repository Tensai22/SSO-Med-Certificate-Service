using MediatR;
using KDS.Primitives.FluentResult;
using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Application.CQRS.Commands;

public record RegisterCommand(
    [property: Required]
    [property: StringLength(120, MinimumLength = 2)]
    string UserName,
    [property: Required]
    [property: EmailAddress]
    string Email,
    [property: Required]
    [property: MinLength(6)]
    string Password,
    [property: Range(1, int.MaxValue)]
    int RoleId) : IRequest<Result<int>>;
