using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.CQRS.Commands;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedicalCertificate.Application.CQRS.Commands;

public class CreateCertificateCommandHandler(
    ICertificateService certificateService,
    ILogger<CreateCertificateCommandHandler> logger)
    : IRequestHandler<CreateCertificateCommand, Result<CertificateDto>>
{
    public async Task<Result<CertificateDto>> Handle(CreateCertificateCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Создание справки: начало обработки запроса");

        var dto = new CertificateDto
        {
            
            UserId = request.UserId,
            Clinic = request.Clinic,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Comment = request.Comment,
            FilePathId = request.FilePathId,
            StatusId = request.StatusId,
            ReviewerComment = request.ReviewerComment,
            CreatedAt = DateTime.UtcNow
        };

        var result = await certificateService.CreateAsync(dto, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogWarning("Ошибка при создании справки: {Error}", result.Error);
            return Result.Failure<CertificateDto>(result.Error);
        }

        logger.LogInformation("Справка успешно создана (ID: {Id})", result.Value.Id);
        return result;
    }
}