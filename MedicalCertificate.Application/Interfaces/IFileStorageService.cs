namespace MedicalCertificate.Application.Interfaces;

public interface IFileStorageService
{
    Task UploadAsync(string objectName, Stream fileStream, string contentType);
    Task<Stream> DownloadAsync(string objectName);
    Task MarkAsDeletedAsync(string objectName);

}