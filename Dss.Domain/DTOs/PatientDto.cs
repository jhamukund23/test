namespace Dss.Domain.DTOs;
public class PatientDto
{
    public string name { get; set; }
    public string? address { get; set; }
    public string? city { get; set; }
    public float age { get; set; }
    public string? gender { get; set; }
}