using MediatR;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public class ApproveCertificateCommand(int certificateId, int approvedByUserId) : IRequest<Result>
{
    public int CertificateId { get; } = certificateId;
    public int ApprovedByUserId { get; } = approvedByUserId;
}