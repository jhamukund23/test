using System.Linq.Expressions;

namespace Dss.Application.Common.Interfaces;

public interface IRepositoryBase<T>
{
    Task<IEnumerable<T>> FindAllAsync();
    Task<IEnumerable<T>> FindByConditionAync(Expression<Func<T, bool>> expression);
    Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
    //Task<bool> Exists(Expression<Func<T, bool>> expression);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();
    Task<int> Save();
}