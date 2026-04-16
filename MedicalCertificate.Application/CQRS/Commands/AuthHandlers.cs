using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MedicalCertificate.Application.DTOs;

namespace MedicalCertificate.Application.CQRS.Handlers;

public class RegisterCommandHandler(IUserRepository userRepository)
    : IRequestHandler<RegisterCommand, Result<int>>
{
    public async Task<Result<int>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result.Failure<int>(new Error("Auth.DuplicateEmail", "User already exists"));
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            RoleId = request.RoleId,
            IIN = "123456789012",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await userRepository.AddAsync(user);
        return Result.Success(user.Id);
    }
}

public class LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

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
            RoleName = user.Role?.Name ?? "Student"
        });
    }
}