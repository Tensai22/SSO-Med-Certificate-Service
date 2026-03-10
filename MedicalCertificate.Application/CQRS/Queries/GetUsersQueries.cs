using MedicalCertificate.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.Application.CQRS.Queries
{
    public class GetUsersQuery : IRequest<Result<UserDto[]>>
    {
    }
}