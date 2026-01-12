using Apolon.Core.Models;

namespace Apolon.Core.Services
{
    public interface ICheckupService
    {
        Task<Checkup?> GetCheckupByIdAsync(int id);
        Task<IEnumerable<Checkup>> GetAllCheckupsAsync();

        Task<int> CreateCheckupAsync(Checkup entity);
        Task UpdateCheckupAsync(Checkup entity);
        Task DeleteCheckupAsync(int id);
    }
}
