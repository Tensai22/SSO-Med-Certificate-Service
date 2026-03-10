using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;


namespace MedicalCertificate.Application.Services;

public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateStatusHistoryRepository _certificateStatusHistoryRepository;
        public CertificateService(
            ICertificateRepository certificateRepository,
            ICertificateStatusHistoryRepository certificateStatusHistoryRepository
            )
        {
            _certificateRepository = certificateRepository;
            _certificateStatusHistoryRepository = certificateStatusHistoryRepository;
        }

        public async Task<Result<CertificateDto>> CreateAsync(CertificateDto dto, CancellationToken cancellationToken)
        {
            var certificate = new Certificate
            {
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Clinic = dto.Clinic,
                Comment = dto.Comment,
                FilePathId = dto.FilePathId,
                StatusId = dto.StatusId,
                ReviewerComment = dto.ReviewerComment,
                CreatedAt = dto.CreatedAt
            };

            await _certificateRepository.AddAsync(certificate);
            await _certificateRepository.SaveChangesAsync();

            var created = await _certificateRepository.GetByIdAsync(certificate.Id);
            if (created == null)
                return Result.Failure<CertificateDto>(new Error(ErrorCode.NotFound, "Ошибка при получении созданной справки."));

            dto.Id = created.Id;
            return dto;
        }

        public async Task<Result<CertificateDto?>> GetByIdAsync(int id)
        {
            var certificate = await _certificateRepository.GetByIdAsync(id);

            if (certificate == null)
                return Result.Failure<CertificateDto?>(new Error(ErrorCode.NotFound, $"Cправка с ID {id} не найдена."));

            var dto = new CertificateDto
            {
                Id = certificate.Id,
                UserId = certificate.UserId,
                StartDate = certificate.StartDate,
                EndDate = certificate.EndDate,
                Clinic = certificate.Clinic,
                Comment = certificate.Comment,
                FilePathId = certificate.FilePathId,
                StatusId = certificate.StatusId,
                ReviewerComment = certificate.ReviewerComment,
                CreatedAt = certificate.CreatedAt
            };

            return dto;
        }

        public async Task<Result<CertificateDto[]>> GetAllAsync()
        {
            var certificates = await _certificateRepository.GetAllAsync();

            if (!certificates.Any())
                return Result.Failure<CertificateDto[]>(new Error(ErrorCode.NotFound, "Справок нет."));

            var result = certificates.Select(certificate => new CertificateDto
            {
                Id = certificate.Id,
                UserId = certificate.UserId
                
            }).ToArray();

            return result;
        }

        public async Task<Result<CertificateDto>> UpdateAsync(int id, CertificateDto dto)
        {
            var certificate = await _certificateRepository.GetByIdAsync(id);

            if (certificate == null)
                return Result.Failure<CertificateDto>(new Error(ErrorCode.NotFound, $"Справка с ID {id} не найдена."));

            certificate.UserId = dto.UserId;
            certificate.StartDate = dto.StartDate;
            certificate.EndDate = dto.EndDate;
            certificate.Clinic = dto.Clinic;
            certificate.Comment = dto.Comment;
            certificate.FilePathId = dto.FilePathId;
            certificate.StatusId = dto.StatusId;
            certificate.ReviewerComment = dto.ReviewerComment;
            certificate.CreatedAt = dto.CreatedAt;
            
            await _certificateRepository.SaveChangesAsync();

            var updated = await _certificateRepository.GetByIdAsync(id);
            if (updated == null)
                return Result.Failure<CertificateDto>(new Error(ErrorCode.NotFound, "Ошибка при получении обновлённой справки."));

            dto.Id = updated.Id;
            return dto;
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var certificate = await _certificateRepository.GetByIdAsync(id);

            if (certificate == null)
                return Result.Failure<bool>(new Error(ErrorCode.NotFound, $"Справка с ID {id} не найдена."));

            await _certificateRepository.RemoveAsync(certificate);
            await _certificateRepository.SaveChangesAsync();

            return true;
        }
        
        public async Task<Result> ApproveAsync(int certificateId, int approvedByUserId, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);

            if (certificate is null)
                return Result.Failure(new Error(ErrorCode.NotFound, $"Справка с ID {certificateId} не найдена."));

            if (certificate.StatusId is 2) 
                return Result.Failure(new Error(ErrorCode.Validation, "Справка уже подтверждена."));

            certificate.StatusId = 2; 
            certificate.ReviewerComment = "Одобрено пользователем " + approvedByUserId;
            
            await _certificateStatusHistoryRepository.AddAsync(new CertificateStatusHistory
            {
                CertificateId = certificateId,
                StatusId = certificate.StatusId,
                ChangedBy = approvedByUserId,
                Comment = "Справка подверждена"
            });
    
            await _certificateRepository.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result> RejectAsync(int certificateId, int rejectedByUserId, string comment, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);

            if (certificate is null)
                return Result.Failure(new Error(ErrorCode.NotFound, $"Справка с ID {certificateId} не найдена."));

            if (certificate.StatusId is 3) 
                return Result.Failure(new Error(ErrorCode.Validation, "Справка уже отклонена."));

            certificate.StatusId = 3; 
            certificate.ReviewerComment = comment;
            
            await _certificateStatusHistoryRepository.AddAsync(new CertificateStatusHistory
            {
                CertificateId = certificate.Id,
                StatusId = 3,
                ChangedBy = rejectedByUserId,
                ChangedAt = DateTime.UtcNow,
                Comment = comment
            });
            
            await _certificateRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
