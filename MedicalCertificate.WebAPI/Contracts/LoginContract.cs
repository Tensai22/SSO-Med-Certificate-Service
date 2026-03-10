using System.Text.Json.Serialization;

namespace MedicalCertificate.WebAPI.Contracts;

public class LoginContract
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
        
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}