using KDS.Primitives.FluentResult;
using MediatR;
using MedicalCertificate.Application.DTOs;

namespace MedicalCertificate.Application.CQRS.Queries;

public class GetCertificateHistoryByCertificateIdQuery(int certificateId) : IRequest<Result<List<CertificateStatusHistoryDto>>>
{
    public int CertificateId { get; init; } = certificateId;
}