using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Queries;
public class GetCertificateByIdQueryHandler(ICertificateService certificateService) : IRequestHandler<GetCertificateByIdQuery, Result<CertificateDto?>>
{
    public async Task<Result<CertificateDto?>> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await certificateService.GetByIdAsync(request.Id);

        if(result.IsFailed)
                return Result.Failure<CertificateDto?>(result.Error);
        return result;
    }
}