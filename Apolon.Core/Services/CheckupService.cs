using Apolon.Core.Models;
using Apolon.Core.ORM.Data;

namespace Apolon.Core.Services
{
    public class CheckupService : ICheckupService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Checkup> _repository;

        public CheckupService(IUnitOfWork uow, IGenericRepository<Checkup> repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<int> CreateCheckupAsync(Checkup entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                await _uow.Repository<Checkup>().AddAsync(entity);

                await _uow.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteCheckupAsync(int id)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Checkup>().DeleteAsync(id);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Checkup>> GetAllCheckupsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Checkup?> GetCheckupByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateCheckupAsync(Checkup entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Checkup>().UpdateAsync(entity);
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
