using KDS.Primitives.FluentResult;
using MediatR;


namespace MedicalCertificate.Application.CQRS.Commands;

public record DeleteCertificateCommand(int id) : IRequest<Result<bool>>;