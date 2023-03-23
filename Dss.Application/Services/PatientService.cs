using Dss.Application.Common.Interfaces;
using Dss.Application.Interfaces;
using Dss.Domain.Models;
using Dss.Infrastructure.Persistence;

public class PatientService : RepositoryBase<Patient>, IPatientService
    {
        public PatientService(ApplicationDBContext context) : base(context)
        { }
        public async Task<string> AddPatientRecordAsync(Patient patient)
        {
            Create(patient);
            await SaveAsync();
            return patient.id.ToString();
        }

        public async Task<IList<Patient>> GetPatientRecordsAsync()
        {
            var res = await FindAllAsync();
            return res.OrderBy(x => x.name).ToList();
        }

        public async Task<Patient?> GetPatientSingleRecordAsync(string id)
        {
            var res = await FindByConditionAync(o => o.id.Equals(id));
            return res.FirstOrDefault();
        }
       
        public async Task<bool> UpdatePatientRecordAsync(Patient model)
        {
            Update(model);
            int rowsAffected = await Save();
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeletePatientRecordAsync(Patient model)
        {
            Delete(model);
            int rowsAffected = await Save();
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }