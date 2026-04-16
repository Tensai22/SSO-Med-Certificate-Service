using MediatR;
using KDS.Primitives.FluentResult;
using MedicalCertificate.Application.DTOs;

namespace MedicalCertificate.Application.CQRS.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponseDto>>;

public record LoginResponse(string Token, int RoleId);