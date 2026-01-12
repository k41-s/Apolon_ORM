using Apolon.Core.Models;

namespace Apolon.Core.Services
{
    public interface IPrescriptionService
    {
        Task<int> CreatePrescriptionAsync(Prescription entity);
        Task<Prescription?> GetPrescriptionByIdAsync(int id);
        Task UpdatePrescriptionAsync(Prescription entity);
        Task DeletePrescriptionAsync(int id);
        Task<IEnumerable<Prescription>> GetAllWithDetailsAsync();
    }
}
