using Apolon.Core.Models;

namespace Apolon.Core.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<object>> GetAllPatientsForSelectListAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        Task<int> CreatePatientAsync(Patient entity);
        Task UpdatePatientAsync(Patient entity);
        Task DeletePatientAsync(int id);
    }
}
