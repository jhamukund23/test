using AutoFixture;
using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Dss.application.Interfaces;
using Dss.API.AzureBlobStorage.Controllers;

namespace dss.unittest;

[TestFixture]
public class StorageControllerUnitTest
{
    private StorageController _storageController;
    private Mock<IAzureStorage> _azureStorage;
    private Mock<ILogger<StorageController>> _logger;
    private Fixture _fixture;
    private readonly string _storageConnectionString;
    private readonly string _storageContainerName;

    public StorageControllerUnitTest(IConfiguration configuration)
    {
        _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
        _storageContainerName = configuration.GetValue<string>("BlobContainerName");
    }
    [SetUp]
    public void Setup()
    {
        _azureStorage = new Mock<IAzureStorage>();
        _logger = new Mock<ILogger<StorageController>>();
        _fixture = new Fixture();
        _storageController = new StorageController(_azureStorage.Object, _logger.Object);
    }




}
