using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.Mapping;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MedicalCertificate.Application.DTOs;

namespace MedicalCertificate.Application.CQRS.Handlers;

// TODO(copilot): auth command handlers still build Edu-backed profiles; prune this path when the schema is simplified.
public class RegisterCommandHandler(
    IUserRepository userRepository,
    IEduUserRepository eduUserRepository,
    IRepository<Edu_Students> eduStudentRepository)
    : IRequestHandler<RegisterCommand, Result<int>>
{
    public async Task<Result<int>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result.Failure<int>(new Error("Auth.DuplicateEmail", "User already exists"));
        }

        var eduUser = await eduUserRepository.GetByEmailWithRelationsAsync(request.Email);
        if (eduUser is null)
        {
            var lastName = request.Email.Split('@', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? request.Email;

            eduUser = new Edu_Users
            {
                LastName = lastName,
                Email = request.Email,
                LastUpdatedBy = request.Email,
                LastUpdatedOn = DateTime.UtcNow,
                Resident = false
            };

            await eduUserRepository.AddAsync(eduUser);
        }

        await EnsureStudentProfileAsync(eduUser, request.Email, request.RoleId);

        var user = new User
        {
            Email = request.Email,
            RoleId = request.RoleId,
            IIN = "123456789012",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EduUserId = eduUser.ID
        };

        await userRepository.AddAsync(user);
        return Result.Success(user.Id);
    }

    private async Task EnsureStudentProfileAsync(Edu_Users eduUser, string updatedBy, int roleId)
    {
        if (roleId != RoleIds.Student || eduUser.Student is not null)
        {
            return;
        }

        await eduStudentRepository.AddAsync(new Edu_Students
        {
            StudentID = eduUser.ID,
            NeedsDorm = false,
            AltynBelgi = false,
            Year = 1,
            LastUpdatedBy = updatedBy,
            LastUpdatedOn = DateTime.UtcNow
        });
    }
}

public class LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailWithRoleAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponseDto>(new Error("Auth.InvalidCredentials", "Invalid email or password"));
        }

        var token = jwtProvider.GenerateToken(user);

        return Result.Success(new AuthResponseDto
        {
            Token = token,
            RoleId = user.RoleId,
            UserId = user.Id,
            Email = user.Email,
            EduUserId = user.EduUserId,
            UserName = EduUserMapper.GetDisplayName(user.EduUser),
            RoleName = user.Role?.Name ?? "Student"
        });
    }
}
