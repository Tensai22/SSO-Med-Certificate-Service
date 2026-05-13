using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Mapping;

namespace MedicalCertificate.Application.CQRS.Queries;

public class GetCertificateHistoryByCertificateIdQueryHandler(
    ICertificateStatusHistoryRepository historyRepository,
    IEduUserRepository eduUserRepository,
    IUserRepository userRepository,
    ICertificateStatusRepository statusRepository)
    : IRequestHandler<GetCertificateHistoryByCertificateIdQuery, Result<List<CertificateStatusHistoryDto>>>
{
    public async Task<Result<List<CertificateStatusHistoryDto>>> Handle(GetCertificateHistoryByCertificateIdQuery request, CancellationToken cancellationToken)
    {
        var histories = await historyRepository.GetByCertificateIdAsync(request.CertificateId);
        if (histories == null || histories.Count == 0)
            return Result.Failure<List<CertificateStatusHistoryDto>>(new Error("History.NotFound", "История не найдена"));

        var result = new List<CertificateStatusHistoryDto>(histories.Count);

        foreach (var h in histories.OrderBy(h => h.ChangedAt))
        {
            var user = await eduUserRepository.GetByIdWithRelationsAsync(h.ChangedBy);
            var status = await statusRepository.GetByIdAsync(h.StatusId);
            var changedByUser = EduUserMapper.GetDisplayName(user);

            if (string.IsNullOrWhiteSpace(changedByUser))
            {
                var authUser = await userRepository.GetByIdWithRoleAsync(h.ChangedBy);
                changedByUser = EduUserMapper.GetDisplayName(authUser?.EduUser);

                if (string.IsNullOrWhiteSpace(changedByUser))
                {
                    changedByUser = authUser?.Email ?? $"User {h.ChangedBy}";
                }
            }

            result.Add(new CertificateStatusHistoryDto
            {
                Status = status?.Status ?? $"ID {h.StatusId}",
                ChangedAt = h.ChangedAt,
                ChangedByUser = changedByUser,
                Comment = h.Comment
            });
        }

        return result;
    }
}
