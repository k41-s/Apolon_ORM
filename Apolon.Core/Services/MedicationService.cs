using Apolon.Core.Models;
using Apolon.Core.ORM.Data;

namespace Apolon.Core.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Medication> _repository;

        public MedicationService(IUnitOfWork uow, IGenericRepository<Medication> repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<int> CreateMedicationAsync(Medication entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                await _uow.Repository<Medication>().AddAsync(entity);

                await _uow.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteMedicationAsync(int id)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Medication>().DeleteAsync(id);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Medication?> GetMedicationByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateMedicationAsync(Medication entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Medication>().UpdateAsync(entity);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
