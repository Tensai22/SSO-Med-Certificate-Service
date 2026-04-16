using KDS.Primitives.FluentResult;
using MediatR;
using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.CQRS.Queries
{
    public class GetCertificatesByUserIdHandler(ICertificateRepository repository)
        : IRequestHandler<GetCertificatesByUserIdQuery, Result<List<CertificateDto>>>
    {
        public async Task<Result<List<CertificateDto>>> Handle(GetCertificatesByUserIdQuery request, CancellationToken ct)
        {
            var certificates = await repository.GetByUserIdAsync(request.UserId);

            // Маппим сущности в DTO (пример)
            var dtos = certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                Clinic = c.Clinic,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                StatusId = c.StatusId,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
