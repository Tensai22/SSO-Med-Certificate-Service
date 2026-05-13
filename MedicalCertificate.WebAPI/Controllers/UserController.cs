using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.WebAPI.Contracts;
using MedicalCertificate.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.RegistrarOnly)]
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
    public async Task<IActionResult> GetByEmail(string email)
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
        var lastUpdatedBy = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(lastUpdatedBy))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new CreateUserCommand(
            lastName: createUserRequest.LastName,
            firstName: createUserRequest.FirstName,
            middleName: createUserRequest.MiddleName,
            email: createUserRequest.Email,
            personalEmail: createUserRequest.PersonalEmail,
            dob: createUserRequest.DOB,
            placeOfBirth: createUserRequest.PlaceOfBirth,
            male: createUserRequest.Male,
            homePhone: createUserRequest.HomePhone,
            mobilePhone: createUserRequest.MobilePhone,
            iin: createUserRequest.IIN,
            photoFileName: createUserRequest.PhotoFileName,
            photoFileData: createUserRequest.PhotoFileData,
            fileContainerId: createUserRequest.FileContainerID,
            mobilePushId: createUserRequest.MobilePushID,
            oldId: createUserRequest.oldId,
            esuvoId: createUserRequest.ESUVOID,
            extraFileContainerId: createUserRequest.ExtraFileContainerID,
            resident: createUserRequest.Resident,
            heroPersonId: createUserRequest.Hero_Person_ID,
            isReadTeamsNotif: createUserRequest.IsReadTeamsNotif,
            nationalityId: createUserRequest.NationalityID,
            maritalStatusId: createUserRequest.MaritalStatusID,
            messengerTypeId: createUserRequest.MessengerTypeID,
            citizenshipCountryId: createUserRequest.CitizenshipCountryID,
            citizenCategoryId: createUserRequest.CitizenCategoryID,
            lastUpdatedBy: lastUpdatedBy));

        if (result.IsFailed)
        {
            return GenerateProblemResponse(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateUserCommand command)
    {
        var lastUpdatedBy = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(lastUpdatedBy))
        {
            return Unauthorized();
        }

        using (logger.BeginScope(new Dictionary<string, string> { { "LastName", command.LastName } }))
        {
            var updatedCommand = command with { Id = id, LastUpdatedBy = lastUpdatedBy };
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
