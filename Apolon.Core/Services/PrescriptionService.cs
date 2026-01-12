using Apolon.Core.Models;
using Apolon.Core.ORM.Data;

namespace Apolon.Core.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Prescription> _repository;

        public PrescriptionService(IUnitOfWork uow, IGenericRepository<Prescription> repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<int> CreatePrescriptionAsync(Prescription entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Prescription>().AddAsync(entity);
                await _uow.CommitAsync();
                return entity.Id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeletePrescriptionAsync(int id)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Prescription>().DeleteAsync(id);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Prescription>> GetAllWithDetailsAsync()
        {
            return await _repository.GetWithRelationsAsync(
                ["Patient", "Medication"]
            );
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdatePrescriptionAsync(Prescription entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Prescription>().UpdateAsync(entity);
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
