using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Commands; 

public record DeleteUserCommand(int Id) : IRequest<Result<bool>>;
