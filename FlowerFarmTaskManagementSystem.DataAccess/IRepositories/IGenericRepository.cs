using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.DataAccess.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Get(
    Expression<Func<T, bool>> filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
    string includeProperties = "",
    int? pageIndex = null,
    int? pageSize = null);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        T GetByID(object id);
    }
}
