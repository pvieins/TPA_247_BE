using System.Linq.Expressions;
using System.Transactions;

namespace PVI.Repository.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetById(string id);
    Task<T> GetById(decimal id);
    T GetByIdNoAsync(decimal id);
    T GetByIdNoAsyncPias(decimal id);
    // Sử dụng đối với những entity có composite key, ví dụ như điểm trực.
    // Sử dụng trường PrKey cho id1 và trường khác cho Id2 (ví dụ mã điểm trực)
    T GetByIdNoAsyncCompKey (int id1, string id2);
    Task<T> GetEntityByCondition(Expression<Func<T, bool>> predicate);
    Task<T> GetEntityByConditionPias(Expression<Func<T, bool>> predicate);
    T GetEntityByConditionNoAsync(Expression<Func<T, bool>> predicate);
    T GetEntityByConditionNoAsyncPias(Expression<Func<T, bool>> predicate);
    Task<T> GetById(int id);
    Task<IEnumerable<T>> GetAll();
    Task Add(T entity);
    IQueryable<T> FindAll();
    void Delete(T entity);
    void DeletePias(T entity);
    void DeleteAll(IEnumerable<T> entity);
    void Update(T entity);
    Task<bool> CheckExists(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetListEntityByCondition(Expression<Func<T, bool>> predicate);
    List<T> GetListEntityByConditionNoAsync(Expression<Func<T, bool>> predicate);
    List<T> GetListEntityByConditionNoAsyncPias(Expression<Func<T, bool>> predicate);
    Task SaveAsync();
    Task SaveAsyncPias();
    void Save();
    Task<List<T>> ToListWithNoLockAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default, Expression<Func<T, bool>> expression = null);
    List<T> ToListWithNoLock<T>(IQueryable<T> query, Expression<Func<T, bool>> expression = null);
    T FirstOrDefaultWithNoLock<T>(IQueryable<T> query, Expression<Func<T, bool>> expression = null);
    Task<T> FirstOrDefaultWithNoLockAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default, Expression<Func<T, bool>> expression = null);
}
