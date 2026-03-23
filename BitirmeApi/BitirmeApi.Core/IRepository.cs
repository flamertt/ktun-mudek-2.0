using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Core
{
    public interface IRepository<T> where T:class,new()
    {
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetList(Expression<Func<T, bool>> filter = null);
        T Get(Expression<Func<T, bool>> filter);
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        T AddNew(T entity);
        void UpdateNew(T entity);
        void DeleteNew(T entity);
    }
}
