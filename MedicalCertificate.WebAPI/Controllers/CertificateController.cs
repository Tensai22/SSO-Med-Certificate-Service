using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.Mapping;
using MedicalCertificate.WebAPI.Contracts;
using MedicalCertificate.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CertificateController(
    ILogger<CertificateController> logger,
    IUserRepository userRepository,
    IEduUserRepository eduUserRepository) : BaseController
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> Get([FromQuery] int? statusId)
    {
        var result = await mediator.Send(new GetCertificateQuery(statusId));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetCertificateByIdQuery(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("filters")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> GetFilters()
    {
        var eduUsers = await eduUserRepository.GetAllWithRelationsAsync();
        var educationInfo = eduUsers
            .Select(EduUserMapper.ResolveEducationInfo)
            .ToArray();

        var departments = educationInfo
            .Select(x => x.Department)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToArray();

        var institutes = educationInfo
            .Select(x => x.Institute)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToArray();

        return Ok(new
        {
            departments,
            institutes
        });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var effectiveUserId = userId;

        if (!User.IsRegistrar())
        {
            var currentUserId = await ResolveCurrentEduUserIdAsync();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            effectiveUserId = currentUserId.Value;
        }

        var result = await mediator.Send(new GetCertificatesByUserIdQuery(effectiveUserId));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}/history")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var result = await mediator.Send(new GetCertificateHistoryByCertificateIdQuery(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCertificateRequest request)
    {
        var currentUserId = await ResolveCurrentEduUserIdAsync();
        if (!currentUserId.HasValue)
        {
            return Unauthorized();
        }

        var targetUserId = request.UserId;
        if (!User.IsRegistrar())
        {
            targetUserId = currentUserId.Value;
        }

        var command = new CreateCertificateCommand
        {
            UserId = targetUserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Clinic = request.Clinic,
            Comment = request.Comment,
            FilePathId = request.FilePathId,
            StatusId = request.StatusId,
            ReviewerComment = request.ReviewerComment
        };

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateCertificateRequest request)
    {
        var command = new UpdateCertificateCommand
        {
            Id = id,
            UserId = request.UserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Clinic = request.Clinic,
            Comment = request.Comment,
            FilePathId = request.FilePathId,
            StatusId = request.StatusId,
            ReviewerComment = request.ReviewerComment,
            CreatedAt = request.CreatedAt
        };

        var result = await mediator.Send(command);
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> Approve(int id)
    {
        var actorUserId = await ResolveCurrentEduUserIdAsync();
        if (!actorUserId.HasValue)
        {
            return Unauthorized();
        }

        using (logger.BeginScope(new Dictionary<string, int>
               {
                   { "CertificateId", id },
                   { "ActorUserId", actorUserId.Value }
               }))
        {
            var command = new ApproveCertificateCommand(id, actorUserId.Value);
            var result = await mediator.Send(command);

            if (result.IsFailed)
            {
                return GenerateProblemResponse(result.Error);
            }

            return Ok("Справка подтверждена");
        }
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectCertificateRequest request)
    {
        var actorUserId = await ResolveCurrentEduUserIdAsync();
        if (!actorUserId.HasValue)
        {
            return Unauthorized();
        }

        var command = new RejectCertificateCommand(id, actorUserId.Value, request.Comment);
        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok("Справка отклонена");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCertificateCommand(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok("Справка удалена");
    }

    private async Task<int?> ResolveCurrentEduUserIdAsync()
    {
        var currentAuthUserId = User.GetCurrentUserId();
        if (!currentAuthUserId.HasValue)
        {
            return null;
        }

        var authUser = await userRepository.GetByIdWithRoleAsync(currentAuthUserId.Value);
        if (authUser?.EduUserId.HasValue == true)
        {
            return authUser.EduUserId.Value;
        }

        return User.GetCurrentEduUserId();
    }
}
