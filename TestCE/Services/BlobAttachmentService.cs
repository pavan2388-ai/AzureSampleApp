using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobAttachmentService
{
    private readonly BlobContainerClient _container;

    public BlobAttachmentService(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration)
    {
        var containerName =
            configuration["BlobStorage:ContainerName"]
            ?? throw new InvalidOperationException(
                "BlobStorage:ContainerName is missing.");

        _container =
            blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task UploadAsync(
        Stream stream,
        string blobName,
        string contentType)
    {
        await _container.CreateIfNotExistsAsync();

        var blob = _container.GetBlobClient(blobName);

        await blob.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });
    }

    public async Task<List<AttachmentItem>> GetAllAsync()
    {
        var result = new List<AttachmentItem>();

        await foreach (var item in _container.GetBlobsAsync())
        {
            result.Add(new AttachmentItem
            {
                BlobName = item.Name,
                FileName = item.Metadata != null &&
                           item.Metadata.TryGetValue(
                               "originalfilename", out var name)
                    ? name
                    : item.Name
            });
        }

        return result;
    }

    public async Task<Stream> DownloadAsync(string blobName)
    {
        var blob = _container.GetBlobClient(blobName);
        var response = await blob.DownloadStreamingAsync();

        return response.Value.Content;
    }

    public async Task DeleteAsync(string blobName)
    {
        var blob = _container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync();
    }
}

public class AttachmentItem
{
    public string BlobName { get; set; } = "";
    public string FileName { get; set; } = "";
}