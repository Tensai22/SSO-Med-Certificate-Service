using MediatR;
using KDS.Primitives.FluentResult;
using MedicalCertificate.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MedicalCertificate.Application.CQRS.Commands;

public record LoginCommand(
    [param: Required]
    [param: EmailAddress]
    string Email,
    [param: Required]
    [param: MinLength(6)]
    string Password) : IRequest<Result<AuthResponseDto>>;

public record LoginResponse(string Token, int RoleId);
