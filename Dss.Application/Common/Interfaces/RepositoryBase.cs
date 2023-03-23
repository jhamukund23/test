using Dss.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dss.Application.Common.Interfaces;
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly ApplicationDBContext _context;
    protected RepositoryBase(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
    {
        return await _context
            .Set<T>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<T>> FindByConditionAync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
    }

    public void Create(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }

    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}