using Dss.Domain.Models;
namespace Dss.Application.Interfaces;
public interface IPatientService  
    { 
        Task<IList<Patient>> GetPatientRecordsAsync(); 
        Task<string> AddPatientRecordAsync(Patient model);       
        Task<Patient?> GetPatientSingleRecordAsync(string id);              
        Task<bool> UpdatePatientRecordAsync(Patient model);
        Task<bool> DeletePatientRecordAsync(Patient model);
    }