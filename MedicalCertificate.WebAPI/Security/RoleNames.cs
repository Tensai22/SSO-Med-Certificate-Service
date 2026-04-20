namespace MedicalCertificate.WebAPI.Security;

public static class RoleNames
{
    public const string OfficeRegistrar = "Office Registrar";
    public const string Student = "Student";

    public static readonly string[] RegistrarAliases =
    {
        OfficeRegistrar,
        "Registrar",
        "Регистратор",
        "Регистратура",
        "Office of Registrar",
        "Офис регистратора"
    };

    public static readonly string[] StudentAliases =
    {
        Student,
        "Студент"
    };
}
