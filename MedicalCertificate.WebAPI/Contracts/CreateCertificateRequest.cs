namespace  MedicalCertificate.WebAPI.Contracts;

public record CreateCertificateRequest(
    int UserId,
    DateTime StartDate,
    DateTime EndDate,
    string Clinic,
    string Comment,
    int FilePathId,
    int StatusId,
    string ReviewerComment,
    DateTime CreatedAt);

