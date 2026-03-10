using MediatR;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    int RoleId) : IRequest<Result<int>>;