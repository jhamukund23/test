using Azure.Storage.Blobs;
using Dss.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Dss.application.Interfaces;

public interface IAzureStorage
{
    Task<BlobResponseDto> UploadAsync(IFormFile file);
    Task<BlobResponseDto> UploadWithSASUrlAsync(IFormFile file, string sasUrl);
    Task<BlobResponseDto> UploadAsync(Stream content, string contentType, string fileName);
    Task<BlobDto> DownloadAsync(string blobFilename);
    Task<BlobResponseDto> DeleteAsync(string blobFilename);
    Task<List<BlobDto>> ListAsync();
    Task<Uri> GetServiceSasUriForContainer(string storedPolicyName = null);
    Uri GetServiceSasUriForDirectory(string storedPolicyName = null);
    public Uri GetServiceSasUriForBlob(string storedPolicyName = null);
    Task<BlobContainerClient> CreateContainerAsync(string containerName);

}
