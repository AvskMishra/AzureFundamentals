using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services;


public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobClient;
    public BlobService(BlobServiceClient blobClient)
    {
        _blobClient = blobClient;
    }
    public async Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobModel)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        var httpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders()
        {
            ContentType = file.ContentType
        };
        IDictionary<string, string> metadata = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(blobModel.Title)) 
            metadata.Add("title", blobModel.Title);
        if (!string.IsNullOrEmpty(blobModel.Comment))
            metadata.Add("comment", blobModel.Comment);
        var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders,metadata);

        //a way to remove metadata
        //IDictionary<string, string> removeMetaData = new Dictionary<string, string>();
        //await blobClient.SetMetadataAsync(removeMetaData);

        //another way to remove specific metadata
        //metadata.Remove("title");
        //await blobClient.SetMetadataAsync(metadata);

        if (result != null)
        {
            return true;
        }
        return false;

    }

    public async Task<bool> DeleteBlob(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<List<string>> GetAllBlobs(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobs = blobContainerClient.GetBlobsAsync();
        
        List<string> blobNames = new List<string>();
        await foreach (var blob in blobs)
        {
            blobNames.Add(blob.Name);
        }

        return blobNames;
    }

    public async Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobs = blobContainerClient.GetBlobsAsync();

        List<BlobModel> blobList = new List<BlobModel>();

        await foreach (var blob in blobs)
        {
            var blobClient = blobContainerClient.GetBlobClient(blob.Name);

            BlobModel blobModel = new()
            {
                Uri = blobClient.Uri.AbsoluteUri
            };
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            if (properties.Metadata.ContainsKey("title"))
            {
                blobModel.Title = properties.Metadata["title"];
            }
            if (properties.Metadata.ContainsKey("comment"))
            {
                blobModel.Comment = properties.Metadata["comment"];
            }

            blobList.Add(blobModel);
        }
        return blobList;
    }

    public async Task<string> GetBlob(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        if (blobClient != null)
        {
            return blobClient.Uri.AbsoluteUri;
        }
        return "";

    }
}