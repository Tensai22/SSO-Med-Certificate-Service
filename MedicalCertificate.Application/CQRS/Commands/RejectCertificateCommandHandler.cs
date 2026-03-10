using MedicalCertificate.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Commands;

public class RejectCertificateCommandHandler(
    ICertificateService certificateService,
    ILogger<RejectCertificateCommandHandler> logger)
    : IRequestHandler<RejectCertificateCommand, Result>
{
    public async Task<Result> Handle(RejectCertificateCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Отклонение справки");

        var result = await certificateService.RejectAsync(
            request.CertificateId,
            request.RejectedByUserId,
            request.Comment,
            cancellationToken
        );

        if (result.IsFailed)
        {
            logger.LogWarning("Ошибка при отклонении справки: {Error}", result.Error?.Message);
            return Result.Failure(result.Error);
        }

        logger.LogInformation("Справка отклонена");
        return Result.Success();
    }
}