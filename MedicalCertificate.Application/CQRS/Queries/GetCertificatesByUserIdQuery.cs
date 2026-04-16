using MedicalCertificate.Application.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace MedicalCertificate.Application.CQRS.Queries;

public record GetCertificatesByUserIdQuery(int UserId) : IRequest<Result<List<CertificateDto>>>;