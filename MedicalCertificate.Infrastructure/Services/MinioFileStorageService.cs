using MedicalCertificate.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace MedicalCertificate.Infrastructure.Services;
public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName = "medical-files";

    public MinioFileStorageService()
    {
        _minioClient = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("DI6T3S5CT5SCJ8CYSJ1S", "6t6WpUoNtJjEzBoekMmX0sYL0NIUoBH8+xCsKraU")
            .WithSSL(false)
            .Build();
    }

    public async Task UploadAsync(string objectName, Stream fileStream, string contentType)
    {
        var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs);

        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(mbArgs);
        }

        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putArgs);
    }

    public async Task<Stream> DownloadAsync(string objectName)
    {
        var memoryStream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public Task MarkAsDeletedAsync(string objectName)
    {
        return Task.CompletedTask;
    }
}