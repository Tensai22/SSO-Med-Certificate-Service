using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalCertificate.Application.DTOs
{
    public class UserDto
    {
        public UserDto(string userName, int roleId, string roleName)
        {
            UserName = userName;
            RoleId = roleId;
            RoleName = roleName;
        }

        public UserDto()
        {
            
        }
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}