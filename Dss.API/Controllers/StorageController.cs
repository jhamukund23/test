using Dss.application.Interfaces;
using Dss.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dss.API.AzureBlobStorage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StorageController : ControllerBase
{
    private readonly IAzureStorage _storage;
    private readonly ILogger<StorageController> _logger;
    public StorageController(IAzureStorage storage, ILogger<StorageController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    [HttpGet(nameof(Get))]
    public async Task<IActionResult> Get()
    {
        // Get all files at the Azure Storage Location and return them
        List<BlobDto>? files = await _storage.ListAsync();
        // Returns an empty array if no files are present at the storage container
        return StatusCode(StatusCodes.Status200OK, files);
    }

    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [HttpPost(nameof(Upload))]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var timer = new Stopwatch();
        timer.Start();

        BlobResponseDto? response = await _storage.UploadAsync(file);

        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        string totalTime = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
        _logger.LogInformation("Total time to upload file in blob" + totalTime);

        // Check if we got an error
        if (response.Error == true)
        {
            // We got an error during upload, return an error with details to the client
            return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
        }
        else
        {
            // Return a success message to the client about successfull upload
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }

    [HttpGet(nameof(GetServiceSasUriForContainer))]
    public async Task<IActionResult> GetServiceSasUriForContainer()
    {
        // Get all files at the Azure Storage Location and return them
        Uri sasUrl = await _storage.GetServiceSasUriForContainer();

        // Returns an empty array if no files are present at the storage container
        return StatusCode(StatusCodes.Status200OK, sasUrl);
    }

    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [HttpPost(nameof(UploadWithSASUrl))]
    public async Task<IActionResult> UploadWithSASUrl(IFormFile file, string sasUrl)
    {
        var timer = new Stopwatch();
        timer.Start();

        BlobResponseDto? response = await _storage.UploadWithSASUrlAsync(file, sasUrl);

        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        string totalTime = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
        _logger.LogInformation("Total time to upload file in blob" + totalTime);

        // Check if we got an error
        if (response.Error == true)
        {
            // We got an error during upload, return an error with details to the client
            return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
        }
        else
        {
            // Return a success message to the client about successfull upload
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }

    [HttpGet("{filename}")]
    public async Task<IActionResult> Download(string filename)
    {
        BlobDto? file = await _storage.DownloadAsync(filename);

        // Check if file was found
        if (file == null)
        {
            // Was not, return error message to client
            return StatusCode(StatusCodes.Status500InternalServerError, $"File {filename} could not be downloaded.");
        }
        else
        {
            // File was found, return it to client
            return File(file.Content, file.ContentType, file.Name);
        }
    }

    [HttpDelete("filename")]
    public async Task<IActionResult> Delete(string filename)
    {
        BlobResponseDto response = await _storage.DeleteAsync(filename);

        // Check if we got an error
        if (response.Error == true)
        {
            // Return an error message to the client
            return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
        }
        else
        {
            // File has been successfully deleted
            return StatusCode(StatusCodes.Status200OK, response.Status);
        }
    }

    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [HttpPost(nameof(UploadLargefile))]
    public async Task<IActionResult> UploadLargefile()
    {
        var timer = new Stopwatch();
        timer.Start();
      
        var file = HttpContext.Request.Form.Files[0];
        var stream = file.OpenReadStream();

        BlobResponseDto? response = await _storage.UploadAsync(stream, file.FileName, file.ContentType);

        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        string totalTime = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");

        _logger.LogInformation("Total time to upload file in blob" + totalTime);
        // Check if we got an error
        if (response.Error == true)
        {
            // We got an error during upload, return an error with details to the client
            return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
        }
        else
        {
            // Return a success message to the client about successfull upload
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}

