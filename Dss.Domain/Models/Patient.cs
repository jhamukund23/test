using System.ComponentModel.DataAnnotations;
namespace Dss.Domain.Models;

public class Patient
{
    [Key] 
    public string id { get; set; }
    public string name { get; set; }
    public string? address { get; set; }
    public string? city { get; set; }
    public float age { get; set; }
    public string? gender { get; set; }
}