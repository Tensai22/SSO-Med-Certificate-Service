using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.CQRS.Queries
{
    public class GetUserByIdQueryHandler(IUserService userService) : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
    {
        public async Task<Result<UserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await userService.GetByIdAsync(request.Id);
            
            if (result.IsFailed)
                return Result.Failure<UserDto?>(result.Error);

            return result;
        }
    }
}