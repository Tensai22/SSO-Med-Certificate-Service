using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Domain.Options;
using MedicalCertificate.Application.Mapping;
using KDS.Primitives.FluentResult;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MedicalCertificate.Application.Services
{
    // TODO(copilot): auth is still using EDU-backed display data; remove this bridge when the UI no longer needs it.
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEduUserRepository _eduUserRepository;
        private readonly IRepository<Edu_Students> _eduStudentRepository;
        private readonly IRepository<Edu_Employees> _eduEmployeeRepository;
        private readonly JwtConfigurationOptions _jwtConfig;

        public AuthService(
            IUserRepository userRepository,
            IEduUserRepository eduUserRepository,
            IRepository<Edu_Students> eduStudentRepository,
            IRepository<Edu_Employees> eduEmployeeRepository,
            IOptions<JwtConfigurationOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _eduUserRepository = eduUserRepository;
            _eduStudentRepository = eduStudentRepository;
            _eduEmployeeRepository = eduEmployeeRepository;
            _jwtConfig = jwtOptions.Value;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(loginDto.Email);

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
            var roleId = EduUserMapper.ResolveRoleId(user.EduUser, user.RoleId);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserName = EduUserMapper.GetDisplayName(user.EduUser),
                FullName = EduUserMapper.GetDisplayName(user.EduUser),
                EduUserId = user.EduUserId,
                RoleId = roleId,
                RoleName = EduUserMapper.ResolveRoleName(user.EduUser, user.Role?.Name ?? string.Empty),
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

            if (registerDto.RoleId is not RoleIds.Student and not RoleIds.OfficeRegistrar)
            {
                return Result.Failure<AuthResponseDto>(new Error(ErrorCode.BadRequest, "Unsupported role"));
            }

            string passwordHash = BC.HashPassword(registerDto.Password);

            var eduUser = await _eduUserRepository.GetByEmailWithRelationsAsync(registerDto.Email);
            if (eduUser is null)
            {
                eduUser = new Edu_Users
                {
                    LastName = registerDto.Email.Split('@', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? registerDto.Email,
                    Email = registerDto.Email,
                    LastUpdatedBy = registerDto.Email,
                    LastUpdatedOn = DateTime.UtcNow,
                    Resident = false
                };
                await _eduUserRepository.AddAsync(eduUser);
            }

            await EnsureEduProfileAsync(eduUser, registerDto.Email, registerDto.RoleId);

            var user = new User
            {
                Email = registerDto.Email,
                RoleId = registerDto.RoleId,
                PasswordHash = passwordHash,
                IIN = string.Empty,
                EduUserId = eduUser.ID
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var createdUser = await _userRepository.GetByEmailWithRoleAsync(registerDto.Email) ?? user;
            var token = GenerateJwtToken(createdUser);
            var roleId = EduUserMapper.ResolveRoleId(createdUser.EduUser, createdUser.RoleId);

            return new AuthResponseDto
            {
                Token = token,
                Email = createdUser.Email,
                UserName = EduUserMapper.GetDisplayName(createdUser.EduUser),
                FullName = EduUserMapper.GetDisplayName(createdUser.EduUser),
                EduUserId = createdUser.EduUserId,
                RoleId = roleId,
                RoleName = EduUserMapper.ResolveRoleName(createdUser.EduUser, createdUser.Role?.Name ?? string.Empty),
                UserId = createdUser.Id
            };
        }

        private async Task EnsureEduProfileAsync(Edu_Users eduUser, string updatedBy, int roleId)
        {
            if (roleId == RoleIds.Student)
            {
                if (eduUser.Student is not null)
                {
                    return;
                }

                await _eduStudentRepository.AddAsync(new Edu_Students
                {
                    StudentID = eduUser.ID,
                    NeedsDorm = false,
                    AltynBelgi = false,
                    Year = 1,
                    LastUpdatedBy = updatedBy,
                    LastUpdatedOn = DateTime.UtcNow
                });
                return;
            }

            if (roleId == RoleIds.OfficeRegistrar)
            {
                if (eduUser.Employee is not null)
                {
                    return;
                }

                await _eduEmployeeRepository.AddAsync(new Edu_Employees
                {
                    ID = eduUser.ID,
                    IsAdvisor = false,
                    RoleGroupId = roleId
                });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, EduUserMapper.ResolveRoleName(user.EduUser, user.Role?.Name ?? string.Empty))
            };

            claims.Add(new Claim("roleId", EduUserMapper.ResolveRoleId(user.EduUser, user.RoleId).ToString()));

            if (user.EduUserId.HasValue)
            {
                claims.Add(new Claim("eduUserId", user.EduUserId.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
