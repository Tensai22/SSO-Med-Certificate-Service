using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicalCertificate.Domain.Constants;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Queries
{
    public class GetUsersQueryHandler(IUserService userService) : IRequestHandler<GetUsersQuery, Result<UserDto[]>>
    {

        public async Task<Result<UserDto[]>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await userService.GetAllAsync();
            
            if (result.IsFailed)
                return Result.Failure<UserDto[]>(new Error(ErrorCode.NotFound, "Пользователей нет."));

            return result;
        }
    }
}