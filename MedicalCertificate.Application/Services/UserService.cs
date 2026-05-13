using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.Mapping;
using MedicalCertificate.Domain.Constants;
using MedicalCertificate.Domain.Entities;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.Services;

// TODO(copilot): transition layer between auth users and EDU profiles; remove once the app is Edu-only.
public class UserService : IUserService
    {
        private readonly IEduUserRepository _userRepository;
        private readonly IUserRepository _authUserRepository;
        public UserService(IEduUserRepository userRepository, IUserRepository authUserRepository)
        {
            _userRepository = userRepository;
            _authUserRepository = authUserRepository;
        }

        public async Task<Result<UserDto[]>> GetAllAsync()
        {
            var users = (await _userRepository.GetAllWithRelationsAsync()).Select(MapToDto).ToArray();

            if (users.Length == 0)
            {
                return Result.Failure<UserDto[]>(new Error(ErrorCode.NotFound, "Записей пользователей нет."));
            }

            return users;
        }

        public async Task<Result<UserDto?>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithRelationsAsync(id);

            if (user == null)
            {
                return Result.Failure<UserDto?>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));
            }

          return EduUserMapper.ToUserDto(user);
        }

        public async Task<Result<UserDto?>> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<UserDto?>(new Error(ErrorCode.Validation, "Email обязателен."));
            }

            var user = await _userRepository.GetByEmailWithRelationsAsync(email);

            if (user == null)
            {
                return Result.Failure<UserDto?>(new Error(ErrorCode.NotFound, $"Пользователь с именем {email} не найден."));
            }

            return MapToDto(user);
        }

        public async Task<Result<UserDto>> CreateAsync(UserDto userDto, CancellationToken cancellationToken)
        {
                if (string.IsNullOrWhiteSpace(userDto.LastName))
                {
                    return Result.Failure<UserDto>(new Error(ErrorCode.Validation, "Фамилия обязательна."));
                }

                if (string.IsNullOrWhiteSpace(userDto.LastUpdatedBy))
                {
                    return Result.Failure<UserDto>(new Error(ErrorCode.Validation, "LastUpdatedBy обязателен."));
                }

                if (!string.IsNullOrWhiteSpace(userDto.Email))
                {
                    var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                    if (existingUser is not null)
                    {
                        return Result.Failure<UserDto>(new Error(ErrorCode.Conflict,
                            "Пользователь с таким email уже существует."));
                    }
                }

                var user = MapToEntity(userDto);
                user.LastUpdatedOn = DateTime.UtcNow;

                await _userRepository.AddAsync(user);

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    var authUser = await _authUserRepository.GetByEmailAsync(user.Email);
                    if (authUser is not null)
                    {
                        if (authUser.EduUserId is not null && authUser.EduUserId != user.ID)
                        {
                            return Result.Failure<UserDto>(new Error(ErrorCode.Conflict, "Пользователь уже привязан к другой учетной записи."));
                        }

                        authUser.EduUserId = user.ID;
                        _authUserRepository.Update(authUser);
                        await _authUserRepository.SaveChangesAsync();
                    }
                }

                userDto.Id = user.ID;
                userDto.LastUpdatedOn = user.LastUpdatedOn;

                return Result.Success(userDto);
        }

        public async Task<Result<UserDto>> UpdateAsync(int id, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return Result.Failure<UserDto>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));
            }

            if (!string.IsNullOrWhiteSpace(userDto.Email))
            {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser is not null && existingUser.ID != id)
                {
                    return Result.Failure<UserDto>(new Error(ErrorCode.Conflict, "Пользователь с таким email уже существует."));
                }

                user.Email = userDto.Email;
            }

            user.LastName = userDto.LastName;
            user.FirstName = userDto.FirstName;
            user.MiddleName = userDto.MiddleName;
            user.PersonalEmail = userDto.PersonalEmail;
            user.DOB = userDto.DOB;
            user.PlaceOfBirth = userDto.PlaceOfBirth;
            user.Male = userDto.Male;
            user.HomePhone = userDto.HomePhone;
            user.MobilePhone = userDto.MobilePhone;
            user.IIN = userDto.IIN;
            user.PhotoFileName = userDto.PhotoFileName;
            user.PhotoFileData = userDto.PhotoFileData;
            user.FileContainerID = userDto.FileContainerID;
            user.MobilePushID = userDto.MobilePushID;
            user.oldId = userDto.oldId;
            user.ESUVOID = userDto.ESUVOID;
            user.ExtraFileContainerID = userDto.ExtraFileContainerID;
            user.Resident = userDto.Resident;
            user.Hero_Person_ID = userDto.Hero_Person_ID;
            user.IsReadTeamsNotif = userDto.IsReadTeamsNotif;
            user.NationalityID = userDto.NationalityID;
            user.MaritalStatusID = userDto.MaritalStatusID;
            user.MessengerTypeID = userDto.MessengerTypeID;
            user.CitizenshipCountryID = userDto.CitizenshipCountryID;
            user.CitizenCategoryID = userDto.CitizenCategoryID;
            user.LastUpdatedBy = string.IsNullOrWhiteSpace(userDto.LastUpdatedBy) ? user.LastUpdatedBy : userDto.LastUpdatedBy;
            user.LastUpdatedOn = DateTime.UtcNow;
            
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _userRepository.GetByIdWithRelationsAsync(id);
            if (updatedUser is null)
                return Result.Failure<UserDto>(new Error(ErrorCode.NotFound, "Ошибка при получении обновленного пользователя."));

            return EduUserMapper.ToUserDto(updatedUser);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                return Result.Failure<bool>(new Error(ErrorCode.NotFound, $"Пользователь с ID {id} не найден."));

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        private static UserDto MapToDto(Edu_Users user)
        {
            return EduUserMapper.ToUserDto(user);
        }

        private static Edu_Users MapToEntity(UserDto userDto)
        {
            return new Edu_Users
            {
                ID = userDto.Id,
                LastName = userDto.LastName,
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                Email = userDto.Email,
                PersonalEmail = userDto.PersonalEmail,
                DOB = userDto.DOB,
                PlaceOfBirth = userDto.PlaceOfBirth,
                Male = userDto.Male,
                HomePhone = userDto.HomePhone,
                MobilePhone = userDto.MobilePhone,
                IIN = userDto.IIN,
                PhotoFileName = userDto.PhotoFileName,
                PhotoFileData = userDto.PhotoFileData,
                LastUpdatedBy = userDto.LastUpdatedBy,
                LastUpdatedOn = userDto.LastUpdatedOn,
                FileContainerID = userDto.FileContainerID,
                MobilePushID = userDto.MobilePushID,
                oldId = userDto.oldId,
                ESUVOID = userDto.ESUVOID,
                ExtraFileContainerID = userDto.ExtraFileContainerID,
                Resident = userDto.Resident,
                Hero_Person_ID = userDto.Hero_Person_ID,
                IsReadTeamsNotif = userDto.IsReadTeamsNotif,
                NationalityID = userDto.NationalityID,
                MaritalStatusID = userDto.MaritalStatusID,
                MessengerTypeID = userDto.MessengerTypeID,
                CitizenshipCountryID = userDto.CitizenshipCountryID,
                CitizenCategoryID = userDto.CitizenCategoryID
            };
        }
    }
