using MedicalCertificate.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public class ApproveCertificateCommandHandler(
    ICertificateService certificateService,
    ILogger<ApproveCertificateCommandHandler> logger)
    : IRequestHandler<ApproveCertificateCommand, Result>
{
    public async Task<Result> Handle(ApproveCertificateCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Подтверждение справки");

        var result = await certificateService.ApproveAsync(request.CertificateId, request.ApprovedByUserId, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogWarning("Ошибка при подтверждении справки: {Error}", result.Error?.Message);
            return Result.Failure(result.Error);
        }

        logger.LogInformation("Справка подтверждена");
        return Result.Success();
    }
}