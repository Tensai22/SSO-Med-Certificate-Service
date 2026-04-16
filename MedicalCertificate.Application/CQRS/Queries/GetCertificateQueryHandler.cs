using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Constants;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Queries;
public class GetCertificateQueryHandler(ICertificateService certificateService) : IRequestHandler<GetCertificateQuery, Result<CertificateDto[]>>

{
    public async Task<Result<CertificateDto[]>> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        var result = await certificateService.GetAllAsync();

        if (result.IsFailed)
            return Result.Failure<CertificateDto[]>(new Error(ErrorCode.NotFound, "Справок нет."));

        if (!request.StatusId.HasValue)
        {
            return result;
        }

        var filtered = result.Value
            .Where(certificate => certificate.StatusId == request.StatusId.Value)
            .ToArray();

        return Result.Success(filtered);
    }
}
