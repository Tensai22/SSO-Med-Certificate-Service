using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.WebAPI.Contracts;
using MedicalCertificate.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.OfficeRegistrar)]
[Route("api/[controller]")]
public class UserController(ILogger<UserController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetUsersQuery());
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByUsername(string email)
    {
        var result = await mediator.Send(new GetUserByEmailQuery(email));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest createUserRequest)
    {
        var result = await mediator.Send(new CreateUserCommand(
            createUserRequest.UserName,
            createUserRequest.Email,
            createUserRequest.Password,
            createUserRequest.RoleId,
            createUserRequest.IIN));

        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateUserCommand command)
    {
        using (logger.BeginScope(new Dictionary<string, string> { { "UserName", command.UserName } }))
        {
            var updatedCommand = command with { Id = id };
            var result = await mediator.Send(updatedCommand);

            if (result.IsFailed)
            {
                return GenerateProblemResponse(result.Error);
            }

            return Ok(result.Value);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteUserCommand(id));
        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return Ok(result.Value);
    }
}
