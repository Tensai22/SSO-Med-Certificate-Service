using MedicalCertificate.Application.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using KDS.Primitives.FluentResult;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { UserId = result.Value });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return Unauthorized(result.Error);
        }

        return Ok(new { Token = result.Value });
    }
}