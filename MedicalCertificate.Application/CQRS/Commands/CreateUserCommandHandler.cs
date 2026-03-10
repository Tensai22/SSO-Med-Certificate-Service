using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedicalCertificate.Application.CQRS.Commands;

public class CreateUserCommandHandler(IUserService userService,ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Отправил запрос на создание клиента");
        var result = await userService.CreateAsync(new(request.UserName, request.RoleId, string.Empty),cancellationToken);

        if (result.IsFailed)
        {
            logger.LogWarning("Возникла ошибка");
            return Result.Failure<UserDto>(result.Error);
        }

        logger.LogInformation("Клиент создан");
        return result;
    }
}