using Microsoft.AspNetCore.Mvc;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    private readonly IFileRepository _fileRepository;

    public FileController(IFileStorageService fileStorage, IFileRepository fileRepository)
    {
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest("Файл не выбран");

        var safeFileName = Path.GetFileName(request.File.FileName);

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        await _fileStorage.UploadAsync(safeFileName, memoryStream, request.File.ContentType);

        var storedFile = new StoredFile
        {
            Name = safeFileName,
            ContentType = request.File.ContentType,
            FileType = Path.GetExtension(request.File.FileName),
            Size = request.File.Length,
            Bucket = "medical-files",
            ObjectKey = safeFileName,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _fileRepository.CreateAsync(storedFile);

        return Ok(new { id = storedFile.Id, name = safeFileName });
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var stream = await _fileStorage.DownloadAsync(fileName);
        return File(stream, "application/octet-stream", fileName);
    }

    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> Delete(string fileName)
    {
        var file = await _fileRepository.GetByNameAsync(fileName);
        if (file == null)
            return NotFound("Файл не найден");

        file.IsDeleted = true;
        await _fileRepository.UpdateAsync(file);

        return Ok($"Файл {fileName} помечен как удалён");
    }

}
