using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Entities;
using MedicalCertificate.Infrastructure.Services;
using MedicalCertificate.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private static readonly HashSet<string> AllowedPdfContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/x-pdf",
        "application/octet-stream"
    };

    private readonly IFileStorageService _fileStorage;
    private readonly IFileRepository _fileRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly MinioOptions _minioOptions;

    public FileController(
        IFileStorageService fileStorage,
        IFileRepository fileRepository,
        ICertificateRepository certificateRepository,
        IOptions<MinioOptions> minioOptions)
    {
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
        _certificateRepository = certificateRepository;
        _minioOptions = minioOptions.Value;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("Файл не выбран");
        }

        var safeFileName = Path.GetFileName(request.File.FileName);
        var fileExtension = Path.GetExtension(safeFileName);
        var isPdfExtension = string.Equals(fileExtension, ".pdf", StringComparison.OrdinalIgnoreCase);
        var isAllowedContentType = AllowedPdfContentTypes.Contains(request.File.ContentType);

        if (!isPdfExtension || !isAllowedContentType)
        {
            return BadRequest("Допускаются только PDF файлы");
        }

        var objectKey = $"{Guid.NewGuid():N}.pdf";

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        await _fileStorage.UploadAsync(objectKey, memoryStream, "application/pdf");

        var storedFile = new StoredFile
        {
            Name = safeFileName,
            ContentType = "application/pdf",
            FileType = ".pdf",
            Size = request.File.Length,
            Bucket = string.IsNullOrWhiteSpace(_minioOptions.BucketName) ? "medical-files" : _minioOptions.BucketName,
            ObjectKey = objectKey,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _fileRepository.CreateAsync(storedFile);

        return Ok(new { id = storedFile.Id, name = safeFileName });
    }

    [HttpGet("download/{fileName}")]
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
    public async Task<IActionResult> Download(string fileName)
    {
        var stream = await _fileStorage.DownloadAsync(fileName);
        return File(stream, "application/octet-stream", fileName);
    }

    [HttpDelete("delete/{fileName}")]
    [Authorize(Roles = RoleNames.OfficeRegistrar)]
    public async Task<IActionResult> Delete(string fileName)
    {
        var file = await _fileRepository.GetByNameAsync(fileName);
        if (file == null)
        {
            return NotFound("Файл не найден");
        }

        file.IsDeleted = true;
        await _fileRepository.UpdateAsync(file);

        return Ok($"Файл {fileName} помечен как удалён");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFileById(int id)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null || file.IsDeleted)
        {
            return NotFound("Файл не найден");
        }

        if (!User.IsInRole(RoleNames.OfficeRegistrar))
        {
            var userId = User.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var certificate = await _certificateRepository.GetByFilePathIdAsync(id);
            if (certificate == null || certificate.UserId != userId.Value)
            {
                return Forbid();
            }
        }

        var stream = await _fileStorage.DownloadAsync(file.ObjectKey);
        return File(stream, file.ContentType ?? "application/octet-stream", enableRangeProcessing: true);
    }
}
