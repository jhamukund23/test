using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Sas;
using Dss.application.Interfaces;
using Dss.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Dss.Application.Services;

public class AzureStorage : IAzureStorage
{
    #region Dependency Injection / Constructor
    private readonly BlobContainerClient _blobContainerClient;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureStorage> _logger;
    private readonly IConfiguration _configuration;
    public AzureStorage(BlobContainerClient blobContainerClient, BlobServiceClient blobServiceClient,IConfiguration configuration, ILogger<AzureStorage> logger)
    {
        _blobContainerClient = blobContainerClient;
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _configuration = configuration;
    }

    #endregion
    public async Task<List<BlobDto>> ListAsync()
    {
        // Create a new list object for 
        List<BlobDto> files = new List<BlobDto>();

        await foreach (BlobItem file in _blobContainerClient.GetBlobsAsync())
        {
            // Add each file retrieved from the storage container to the files list by creating a BlobDto object
            string uri = _blobContainerClient.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType,
                LastModifiedDate = file.Properties.LastModified
            });
        }

        // Return all files to the requesting method
        return files;
    }
    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();

        // Get a reference to a container named in appsettings.json and then create it
        
        await _blobContainerClient.CreateIfNotExistsAsync();
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = _blobContainerClient.GetBlobClient(blob.FileName);

            // Set or update blob index tags on existing blob
            var Tags = new Dictionary<string, string> {
                { "uploaddate", DateTimeOffset.UtcNow.ToString() },
                { "customername", "MRM" }
                };

            // Specify the StorageTransferOptions
            BlobUploadOptions options = new BlobUploadOptions
            {
                TransferOptions = new StorageTransferOptions
                {
                    // Set the maximum number of workers that 
                    // may be used in a parallel transfer.
                    InitialTransferSize = 1 * 1024 * 1024,
                    MaximumConcurrency = 8,
                    // Set the maximum length of a transfer to 10MB.
                    MaximumTransferSize = 10 * 1024 * 1024
                },
                Tags = Tags
            };

            //var containerASAUri = GetServiceSasUriForContainer(container);
            //var blobASAUri = GetServiceSasUriForBlob(client);

            // Open a stream for the file we want to upload
            await using (Stream? data = blob.OpenReadStream())
            {
                // Upload the file async
                await client.UploadAsync(data, options);
            }

            // Everything is OK and file got uploaded
            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
           when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError($"File with name {blob.FileName} already exists in container. Set another name to store the file in the container: '{_blobContainerClient.Name}.'");
            response.Status = $"File with name {blob.FileName} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;
    }

    public async Task<BlobResponseDto> UploadWithSASUrlAsync(IFormFile blob, string sasUrl)
    {
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();

        Uri sasUrlForContainer = new Uri(sasUrl);

        // Get a reference to a container named in appsettings.json and then create it
        BlobContainerClient container = new BlobContainerClient(sasUrlForContainer);
        //await container.CreateIfNotExistsAsync();
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = container.GetBlobClient(blob.FileName);

            // Set or update blob index tags on existing blob
            var Tags = new Dictionary<string, string> {
                { "uploaddate", DateTimeOffset.UtcNow.ToString() },
                { "customername", "MRM" }
                };

            // Specify the StorageTransferOptions
            BlobUploadOptions options = new BlobUploadOptions
            {
                TransferOptions = new StorageTransferOptions
                {
                    // Set the maximum number of workers that 
                    // may be used in a parallel transfer.
                    InitialTransferSize = 1 * 1024 * 1024,
                    MaximumConcurrency = 8,
                    // Set the maximum length of a transfer to 10MB.
                    MaximumTransferSize = 10 * 1024 * 1024
                },
                Tags = Tags
            };

            //var containerASAUri = GetServiceSasUriForContainer(container);
            //var blobASAUri = GetServiceSasUriForBlob(client);

            // Open a stream for the file we want to upload
            await using (Stream? data = blob.OpenReadStream())
            {
                // Upload the file async
                await client.UploadAsync(data, options);
            }

            // Everything is OK and file got uploaded
            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
           when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError($"File with name {blob.FileName} already exists in container. Set another name to store the file in the container: '{_blobContainerClient.Name}.'");
            response.Status = $"File with name {blob.FileName} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;
    }
    public async Task<BlobDto> DownloadAsync(string blobFilename)
    {
        try
        {
            // Get a reference to the blob uploaded earlier from the API in the container from configuration settings
            BlobClient file = _blobContainerClient.GetBlobClient(blobFilename);

            // Check if the file exists in the container
            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                // Download the file details async
                var content = await file.DownloadContentAsync();

                // Add data to variables in order to return a BlobDto
                string name = blobFilename;
                string contentType = content.Value.Details.ContentType;

                // Create new BlobDto with blob data from variables
                return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
            }
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            // Log error to console
            _logger.LogError($"File {blobFilename} was not found.");
        }

        // File does not exist, return null and handle that in requesting method
        return null;
    }
    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        BlobClient file = _blobContainerClient.GetBlobClient(blobFilename);

        try
        {
            // Delete the file
            await file.DeleteAsync();
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            // File did not exist, log to console and return new response to requesting method
            _logger.LogError($"File {blobFilename} was not found.");
            return new BlobResponseDto { Error = true, Status = $"File with name {blobFilename} not found." };
        }

        // Return a new BlobResponseDto to the requesting method
        return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };

    }

    public async Task<BlobResponseDto> UploadAsync(Stream stream, string contentType, string fileName)
    {
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();
      
        await _blobContainerClient.CreateIfNotExistsAsync();
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = _blobContainerClient.GetBlobClient(fileName);
            // Specify the StorageTransferOptions
            BlobUploadOptions options = new BlobUploadOptions
            {
                TransferOptions = new StorageTransferOptions
                {
                    // Set the maximum number of workers that 
                    // may be used in a parallel transfer.
                    InitialTransferSize = 1 * 1024 * 1024,
                    MaximumConcurrency = 8,
                    // Set the maximum length of a transfer to 10MB.
                    MaximumTransferSize = 10 * 1024 * 1024
                }
            };
            // Upload the file async
            await client.UploadAsync(stream, options);

            // Everything is OK and file got uploaded
            response.Status = $"File {fileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
           when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError($"File with name {fileName} already exists in container. Set another name to store the file in the container: '{_blobContainerClient.Name}.'");
            response.Status = $"File with name {fileName} already exists. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;

    }

    public async Task<Uri> GetServiceSasUriForContainer(string storedPolicyName = null)
    {
       
        await _blobContainerClient.CreateIfNotExistsAsync();
        // Check whether this BlobContainerClient object has been authorized with Shared Key.
        if (_blobContainerClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _blobContainerClient.Name,
                Resource = "c"
            };

            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow;
                //sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);
                sasBuilder.SetPermissions(BlobSasPermissions.Tag | BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            Uri sasUri = _blobContainerClient.GenerateSasUri(sasBuilder);
            return sasUri;
        }
        else
        {
            return null;
        }
    }
    public Uri GetServiceSasUriForBlob(string storedPolicyName = null)
    {

        BlobClient blobClient = _blobContainerClient.GetBlobClient("Humana Project Total Repo.txt");
        if (_blobContainerClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _blobContainerClient.Name,
                BlobName = "Humana Project Total Repo.txt",
                Resource = "b"
            };
            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow;
                //sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);
                sasBuilder.SetPermissions(BlobSasPermissions.Tag | BlobSasPermissions.Read | BlobSasPermissions.Write);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri;
        }
        else
        {
            return null;
        }
    }


    public async Task<BlobContainerClient> CreateContainerAsync(string containerName)
    {
        // Name the sample container based on new GUID to ensure uniqueness.
        // The container name must be lowercase.       
        try
        {
            // Create the container
            //var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            //await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            BlobContainerClient container = await _blobServiceClient.CreateBlobContainerAsync(containerName);

            if (await container.ExistsAsync())
            {
                _logger.LogInformation("Created container {0}", container.Name);
                return container;
            }
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("HTTP error code {0}: {1}", ex.Status, ex.ErrorCode);
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
        }
        return null;
    }
    public Uri GetServiceSasUriForDirectory(string storedPolicyName = null)
    {
        string path1 = "mr1";

        CreateSubDirectories(_configuration["BlobConnectionString"], path1, "20230324/53453/1");
        string path = "/20230322/1234567/1/";
        DataLakeDirectoryClient directoryClient = new DataLakeDirectoryClient(_configuration["BlobConnectionString"], "mr1", path);

        
        if (directoryClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one hour.
            DataLakeSasBuilder sasBuilder = new DataLakeSasBuilder()
            {
                // Specify the file system name, the path, and indicate that
                // the client object points to a directory.
                FileSystemName = directoryClient.FileSystemName,
                Resource = "d",
                IsDirectory = true,
                Path = directoryClient.Path                
            };

            // If no stored access policy is specified, create the policy
            // by specifying expiry and permissions.
            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(2);
                sasBuilder.SetPermissions(DataLakeSasPermissions.Read |
                                          DataLakeSasPermissions.Write |
                                          DataLakeSasPermissions.List);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            // Get the SAS URI for the specified directory.
            Uri sasUri = directoryClient.GenerateSasUri(sasBuilder);
            return sasUri;
        }
        else
        {
            Console.WriteLine(@"DataLakeDirectoryClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
            return null;
        }
    }

    //Parameters

    //Connectionstring  : Datalake connection string
    //path : "ContainerName/folder1"
    //newFolderName : "folder2"

    public void CreateSubDirectories(string connectionString, string path, string newFolderName)
    {
        var storageAccount = new DataLakeServiceClient(connectionString);
        string[] strPath = path.Split("/");
        string container = string.Empty;
        string newFoldername = string.Empty;
        DataLakeFileSystemClient dirClient = storageAccount.GetFileSystemClient(strPath[0]);
        if (strPath.Length == 1) //Create folder inside container
        {
            DataLakeDirectoryClient directory = dirClient.GetDirectoryClient(newFolderName);
            directory.CreateIfNotExists();
        }
        else
        {
            DataLakeDirectoryClient directory = dirClient.GetDirectoryClient(strPath[1]); //Create folder inside folder in container
            if (strPath.Length == 2)
            {
                directory.CreateSubDirectory(newFolderName);

            }
            else
            {
                //Create folder inside folder in container
                int i = 0;
                foreach (string str in strPath)
                {
                    i++;
                    if (i > 2)
                    {
                        container = container + str + ((i + 1 == strPath.Length) ? "" : "/");
                    }

                }
                DataLakeDirectoryClient subdirectory = directory.GetSubDirectoryClient(container);
                subdirectory.CreateSubDirectory(newFolderName);
            }
        }
    }


}
