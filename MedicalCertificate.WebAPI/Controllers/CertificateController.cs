using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.WebAPI.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MedicalCertificate.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CertificateController(ILogger<CertificateController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetCertificateQuery());
        if (result.IsFailed)
            return GenerateProblemResponse(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetCertificateByIdQuery(id));
        if (result.IsFailed)
            return GenerateProblemResponse(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var result = await mediator.Send(new GetCertificateHistoryByCertificateIdQuery(id));
        if (result.IsFailed)
            return GenerateProblemResponse(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCertificateRequest request)
    {
        var command = new CreateCertificateCommand
        {
            UserId = request.UserId,
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
            return GenerateProblemResponse(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
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
            return GenerateProblemResponse(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveCertificateRequest request)
    {
        var scope = new Dictionary<string, int>()
        {
            { "CertificateId", request.ApprovedByUserId }
        };
        using (logger.BeginScope(scope))
        {
            var command = new ApproveCertificateCommand(id, request.ApprovedByUserId);
            var result = await mediator.Send(command);

            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);

            return Ok("Справка подтверждена");
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectCertificateRequest request)
    {
        var command = new RejectCertificateCommand(id, request.RejectedByUserId, request.Comment);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return GenerateProblemResponse(result.Error);

        return Ok("Справка отклонена");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCertificateCommand(id));
        if (result.IsFailed)
            return GenerateProblemResponse(result.Error);

        return Ok("Справка удалена");
    }
}
