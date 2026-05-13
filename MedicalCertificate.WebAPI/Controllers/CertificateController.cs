using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.WebAPI.Contracts;
using MedicalCertificate.WebAPI.Security;
using MedicalCertificate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CertificateController(
    ILogger<CertificateController> logger,
    IUserRepository userRepository,
    AppDbContext dbContext) : BaseController
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
        var orgUnits = await dbContext.Edu_OrgUnits
            .AsNoTracking()
            .Include(unit => unit.Type)
            .Where(unit => !unit.Deleted && unit.Type != null)
            .Select(unit => new
            {
                id = unit.ID,
                title = unit.Title,
                parentId = unit.ParentID,
                typeTitle = unit.Type!.Title
            })
            .ToListAsync();

        var departments = orgUnits
            .Where(unit => IsOrgUnitType(unit.typeTitle, "кафедра", "Department", "chair"))
            .Select(unit => unit.title?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToArray();

        var institutes = orgUnits
            .Where(unit => IsOrgUnitType(unit.typeTitle, "Институт", "Institute", "instit"))
            .Select(unit => unit.title?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToArray();

        return Ok(new
        {
            orgUnits,
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

    private static bool IsOrgUnitType(string? actualType, params string[] expectedTypes)
    {
        if (string.IsNullOrWhiteSpace(actualType))
        {
            return false;
        }

        return expectedTypes.Any(expected =>
            string.Equals(actualType.Trim(), expected, StringComparison.OrdinalIgnoreCase));
    }
}
