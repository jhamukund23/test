using AutoFixture;
using NUnit.Framework;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Dss.Domain.Models;
using Dss.Application.Interfaces;
using Dss.API.Controllers;

namespace dss.unittest;

[TestFixture]
public class PatientsControllerUnitTest
{
    private PatientsController _patientsController;
    private Mock<IPatientService> _patientService;
    private Mock<IMapper> _mapper;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _patientService = new Mock<IPatientService>();
        _mapper = new Mock<IMapper>();
        _fixture = new Fixture();
        _patientsController = new PatientsController(_patientService.Object, _mapper.Object);
    }

    [Test]
    public async Task GetAll()
    {
         var expected = _fixture.Create<List<Patient>>();
        _patientService.Setup(m => m.GetPatientRecordsAsync()).ReturnsAsync(expected);

       var actual = await _patientsController.GetAll() as OkObjectResult;;

        //Assert          
        Assert.IsNotNull(actual);        
        Assert.AreEqual(200, actual.StatusCode);
        var result = actual.Value as IEnumerable<Patient>;
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Select(g => g.id).Intersect(expected.Select(d => d.id)).Count(),result.Count());
    }

    [Test]
    public async Task Details()
    {
        var patientList = _fixture.Create<IList<Patient>>();
        var patientId = patientList.Take(1).FirstOrDefault().id;
        var expected = patientList.Where(p => p.id == patientId).FirstOrDefault();
        _patientService.Setup(m => m.GetPatientSingleRecordAsync(patientId)).ReturnsAsync(expected);

        var actual = await _patientsController.Details(patientId) as OkObjectResult;

        //Assert          
        Assert.IsNotNull(actual);     
        Assert.AreEqual(200, actual.StatusCode);
        var result = actual.Value as Patient;
        Assert.IsNotNull(result);
        Assert.AreEqual(result.id, expected.id);
    }
}
