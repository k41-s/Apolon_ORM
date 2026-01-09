using Apolon.Core.ORM.Data;
using Npgsql;

namespace Apolon.Core.ORM
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseService _dbService;
        private NpgsqlConnection? _connection;
        private NpgsqlTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        private async Task EnsureConnectionOpenAsync()
        {
            if (_connection == null)
            {
                _connection = await _dbService.GetNewOpenConnectionAsync();
            }
        }

        public async Task BeginTransactionAsync()
        {
            await EnsureConnectionOpenAsync();
            if (_transaction != null) throw new InvalidOperationException("Transaction already started.");
            _transaction = await _connection!.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null) throw new InvalidOperationException("No transaction to commit.");
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync();
            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public IGenericRepository<T> Repository<T>() where T : new()
        {
            EnsureConnectionOpenAsync().GetAwaiter().GetResult();
            return new GenericRepository<T>(_connection!, _transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null) _transaction.Dispose();
                    if (_connection != null) _connection.Dispose();
                }
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null) await _transaction.DisposeAsync();
            if (_connection != null) await _connection.DisposeAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
