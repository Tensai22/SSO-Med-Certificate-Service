using MediatR;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public class RejectCertificateCommand(int certificateId, int rejectedByUserId, string comment) : IRequest<Result>
{
    public int CertificateId { get; init; } = certificateId;
    public int RejectedByUserId { get; init; } = rejectedByUserId;
    public string Comment { get; init; } = comment;
}