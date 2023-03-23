using AutoMapper;
using Dss.Domain.DTOs;
using Dss.Domain.Models;
using Dss.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Dss.API.Controllers;

[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private IMapper _mapper { get; }
    public PatientsController(IPatientService patientService, IMapper mapper)
    {
        _patientService = patientService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _patientService.GetPatientRecordsAsync();
        return result != null ? (IActionResult)Ok(result) : StatusCode(500);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PatientDto patientDto)
    {
        if (ModelState.IsValid)
        {
            Guid obj = Guid.NewGuid();
            var model = _mapper.Map<Patient>(patientDto);
            model.id = obj.ToString();
            await _patientService.AddPatientRecordAsync(model);
            return Ok(new { StatusCode = StatusCodes.Status201Created });
        }
        return StatusCode(StatusCodes.Status400BadRequest);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var result = await _patientService.GetPatientSingleRecordAsync(id);
        return result != null ? (IActionResult)Ok(result) : StatusCode(500);
    }

    [HttpPut]
    public async Task<IActionResult> EditAsync([FromBody] Patient patient)
    {
        if (ModelState.IsValid)
        {
            var details = await _patientService.GetPatientSingleRecordAsync(patient.id);
            if (details != null)
            {
                await _patientService.UpdatePatientRecordAsync(patient);
                return Ok(new { StatusCode = StatusCodes.Status201Created });
            }
            else
            {
                return Ok(new { StatusCode = StatusCodes.Status404NotFound });
            }
        }
        return Ok(new { StatusCode = StatusCodes.Status400BadRequest });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConfirmedAsync(string id)
    {
        var data = await _patientService.GetPatientSingleRecordAsync(id);
        if (data == null)
        {
            return Ok(new { StatusCode = StatusCodes.Status404NotFound });
        }
        else
        {
            await _patientService.DeletePatientRecordAsync(data);
            return Ok(new { StatusCode = StatusCodes.Status201Created });
        }

    }
}
