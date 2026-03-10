namespace MedicalCertificate.WebAPI.Contracts;

public record CreateUserRequest(string UserName, int RoleId);