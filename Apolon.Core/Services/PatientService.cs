using Apolon.Core.Models;
using Apolon.Core.ORM.Data;

namespace Apolon.Core.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Patient> _repository;

        public PatientService(IUnitOfWork uow, IGenericRepository<Patient> repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<int> CreatePatientAsync(Patient entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();

                await _uow.Repository<Patient>().AddAsync(entity);

                await _uow.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeletePatientAsync(int id)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Patient>().DeleteAsync(id);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<object>> GetAllPatientsForSelectListAsync()
        {
            var patients = await _repository.GetAllAsync();

            return patients.Select(p => new
            {
                Id = p.Id,
                FullName = $"{p.FirstName} {p.LastName}"
            }).ToList();
        }

        public async Task UpdatePatientAsync(Patient entity)
        {
            try
            {
                await _uow.BeginTransactionAsync();
                await _uow.Repository<Patient>().UpdateAsync(entity);
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
