using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MediatR;
using MedicalCertificate.WebAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCertificate.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController(ILogger<UserController> logger) : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await mediator.Send(new GetUsersQuery());
            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));
            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);

            return Ok(result);
        }
        
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByUsername(string email)
        {
            var result = await mediator.Send(new GetUserByEmailQuery(email));
            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserRequest createUserRequest)
        {
            var result = await mediator.Send(new CreateUserCommand(createUserRequest.UserName, createUserRequest.RoleId));
            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);
            return Created();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserCommand command)
        {
            Dictionary<string,string> log = new Dictionary<string, string>()
            {
                {  "UserName",command.UserName}
            };
            using (logger.BeginScope(log))
            {

                var updatedCommand = command with { Id = id };
                var result = await mediator.Send(updatedCommand);

                if (result.IsFailed)
                    return GenerateProblemResponse(result.Error);

                return Ok(result);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteUserCommand(id));
            if (result.IsFailed)
                return GenerateProblemResponse(result.Error);
            return Ok(result);
        }
    }
}