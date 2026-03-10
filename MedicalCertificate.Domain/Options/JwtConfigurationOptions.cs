namespace MedicalCertificate.Domain.Options;

public class JwtConfigurationOptions
{
    // днаюбэ щрс ярпнйс:
    public const string SectionName = "JwtConfigurationOptions";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationHours { get; set; }
}