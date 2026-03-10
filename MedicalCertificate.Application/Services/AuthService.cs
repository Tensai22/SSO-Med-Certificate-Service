using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Domain.Options;
using KDS.Primitives.FluentResult;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MedicalCertificate.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfigurationOptions _jwtConfig;

        public AuthService(IUserService userService, IUserRepository userRepository, IOptions<JwtConfigurationOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtConfig = jwtOptions.Value;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return Result.Failure<AuthResponseDto>(new Error(ErrorCode.Unauthorized, "Неверное имя пользователя или пароль"));
            }

            bool isPasswordValid = BC.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Result.Failure<AuthResponseDto>(new Error(ErrorCode.Unauthorized, "Неверное имя пользователя или пароль"));
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                UserId = user.Id
            };
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDTO registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);

            if (existingUser is not null)
            {
                return Result.Failure<AuthResponseDto>(new Error(ErrorCode.Conflict, "Пользователь с таким именем уже существует"));
            }

            string passwordHash = BC.HashPassword(registerDto.Password);

            var user = new User
            {
                Email = registerDto.Email,
                RoleId = registerDto.RoleId,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                UserId = user.Id
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(_jwtConfig.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}