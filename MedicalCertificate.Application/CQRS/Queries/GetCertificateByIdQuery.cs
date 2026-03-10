using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Queries;

public record GetCertificateByIdQuery(int Id) : IRequest<Result<CertificateDto?>>;
