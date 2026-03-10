using MediatR;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<Result<string>>;