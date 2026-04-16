using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.WebAPI.Contracts;
using MedicalCertificate.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CertificateController(ILogger<CertificateController> logger) : BaseController
{
    [HttpGet]
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
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

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var currentUserId = User.GetCurrentUserId();
        if (!User.IsInRole(RoleNames.OfficeRegistrar) && currentUserId != userId)
        {
            return Forbid();
        }

        var result = await mediator.Send(new GetCertificatesByUserIdQuery(userId));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}/history")]
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
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
        var currentUserId = User.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Unauthorized();
        }

        var targetUserId = User.IsInRole(RoleNames.OfficeRegistrar)
            ? request.UserId
            : currentUserId.Value;

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
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
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
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
    public async Task<IActionResult> Approve(int id)
    {
        var actorUserId = User.GetCurrentUserId();
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
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectCertificateRequest request)
    {
        var actorUserId = User.GetCurrentUserId();
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
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCertificateCommand(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok("Справка удалена");
    }
}
