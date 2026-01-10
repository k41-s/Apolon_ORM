using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apolon.Core.ORM.Data
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        IGenericRepository<T> Repository<T>() where T : new();
        Task<int> ExecuteRawSqlAsync(string sql, object[] parameters = null);
    }
}
