using Apolon.Core.Models;

namespace Apolon.Core.Services
{
    public interface IMedicationService
    {
        Task<Medication?> GetMedicationByIdAsync(int id);
        Task<IEnumerable<Medication>> GetAllMedicationsAsync();

        Task<int> CreateMedicationAsync(Medication entity);
        Task UpdateMedicationAsync(Medication entity);
        Task DeleteMedicationAsync(int id);
    }
}
