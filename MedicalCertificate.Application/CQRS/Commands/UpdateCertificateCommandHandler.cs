using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.Services;
using MedicalCertificate.Application.CQRS.Commands;
using KDS.Primitives.FluentResult;
using MediatR;

namespace MedicalCertificate.Application.CQRS.Commands;

public class UpdateCertificateCommandHandler : IRequestHandler<UpdateCertificateCommand, Result<CertificateDto>>
{
    private readonly ICertificateService certificateService;

    public UpdateCertificateCommandHandler(ICertificateService certificateService)
    {
        this.certificateService = certificateService;
    }


    public async Task<Result<CertificateDto>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
    {
        var dto = new CertificateDto
        {
            Id = request.Id,
            UserId = request.UserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Clinic = request.Clinic,
            Comment = request.Comment,
            FilePathId = request.FilePathId,
            StatusId = request.StatusId,
            ReviewerComment = request.ReviewerComment,
            CreatedAt = request.CreatedAt,
        };

        var result = await certificateService.UpdateAsync(request.Id, dto);

        if (result.IsFailed)
            return Result.Failure<CertificateDto>(result.Error);

        return result;
    }
}