using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.DTOs;
public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    // TODO(copilot): keep the EDU-backed display name only until the UI stops depending on it.
    public string UserName { get; set; } = null!;
    public int? EduUserId { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public int UserId { get; set; }
}
    
