using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaxService.Configuration
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<bool> Delete(int id);
        Task<List<T>> FindAsync(Func<T, bool> predicate);
        Task SaveChangesAsync();
    }
}
