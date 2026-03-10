using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;


namespace MedicalCertificate.Application.CQRS.Commands;

public class CreateCertificateCommand : IRequest<Result<CertificateDto>>
{
    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Clinic { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public int FilePathId { get; set; }

    public int StatusId { get; set; }

    public string ReviewerComment { get; set; } = string.Empty;
}