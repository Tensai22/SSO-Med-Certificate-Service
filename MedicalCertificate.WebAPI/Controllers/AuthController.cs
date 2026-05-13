using MedicalCertificate.Application.CQRS.Commands;
using MedicalCertificate.Application.CQRS.Queries;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.Mapping;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MedicalCertificate.WebAPI.Security;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// TODO(copilot): this endpoint still exposes EDU display data for compatibility; trim it in the final integration pass.
public class AuthController(IMediator mediator, IUserRepository userRepository) : ControllerBase
{
    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsFailed) {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var user = await userRepository.GetByIdWithRoleAsync(userId.Value);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            userId = userId.Value,
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role),
            user = new
            {
                user.Id,
                user.Email,
                user.IIN,
                RoleId = MedicalCertificate.Application.Mapping.EduUserMapper.ResolveRoleId(user.EduUser, user.RoleId),
                user.EduUserId,
                userName = EduUserMapper.GetDisplayName(user.EduUser),
                fullName = EduUserMapper.GetDisplayName(user.EduUser),
                EduUser = user.EduUser is null ? null : EduUserMapper.ToUserDto(user.EduUser),
                RoleName = EduUserMapper.ResolveRoleName(user.EduUser, user.Role?.Name ?? string.Empty)
            }
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out" });
    }
}
