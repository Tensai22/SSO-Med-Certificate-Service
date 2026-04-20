using MediatR;
using KDS.Primitives.FluentResult;
using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Application.CQRS.Commands;

public record RegisterCommand(
    [param: Required]
    [param: StringLength(120, MinimumLength = 2)]
    string UserName,
    [param: Required]
    [param: EmailAddress]
    string Email,
    [param: Required]
    [param: MinLength(6)]
    string Password,
    [param: Range(1, int.MaxValue)]
    int RoleId) : IRequest<Result<int>>;
