using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.Services;

public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Result<UserDto[]>> GetAllAsync()
        {
            var users = (await _userRepository.GetAllWithRolesAsync()).Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? string.Empty
            }).ToArray();

            if (users.Length == 0)
            {
                return Result.Failure<UserDto[]>(new Error(ErrorCode.NotFound, "Пользователей нет."));
            }

            return users;
        }

        public async Task<Result<UserDto?>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithRoleAsync(id);

            if (user == null)
            {
                return Result.Failure<UserDto?>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));
            }

          return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty
            };
        }

        public async Task<Result<UserDto?>> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(email);

            if (user == null)
            {
                return Result.Failure<UserDto?>(new Error(ErrorCode.NotFound, $"Пользователь с именем {email} не найден."));
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty
            };
            
            return userDto;
        }

        public async Task<Result<UserDto>> CreateAsync(UserDto userDto, CancellationToken cancellationToken)
        {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser is not null)
                {
                    return Result.Failure<UserDto>(new Error(ErrorCode.Conflict,
                        "Пользователь с таким именем уже существует."));
                }

                var user = new User
                {
                    UserName = userDto.UserName,
                    RoleId = userDto.RoleId
                };

                await _userRepository.AddAsync(user);

                userDto.Id = user.Id;


                return Result.Success(userDto);
        }

        public async Task<Result<UserDto>> UpdateAsync(int id, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return Result.Failure<UserDto>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));
            }

            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser is not null && existingUser.Id != id)
            {
                return Result.Failure<UserDto>(new Error(ErrorCode.Conflict, "Пользователь с таким именем уже существует."));
            }


            user.UserName = userDto.UserName;
            user.RoleId = userDto.RoleId;
            
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _userRepository.GetByIdWithRoleAsync(id);
            if (updatedUser is null)
                return Result.Failure<UserDto>(new Error(ErrorCode.NotFound, "Ошибка при получении обновленного пользователя."));

            return new UserDto
            {
                Id = updatedUser.Id,
                UserName = updatedUser.UserName,
                RoleId = updatedUser.RoleId,
                RoleName = updatedUser.Role?.Name ?? string.Empty
            };
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                return Result.Failure<bool>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));

            user.IsDeleted = true;
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }