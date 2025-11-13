using System.Linq.Expressions;
using BrainBay.Core.Entities;

namespace BrainBay.Core.Repositories
{
    public interface IRepository<T, in TId> where T : class
    {
        Task<T> AddAsync(T entity);

        Task SaveChangesAsync();

        Task UpdateAsync(T entity);

        Task DeleteAsync(TId id);

        Task<T?> GetByIdAsync(TId id);

        Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>>? criteria);

        Task<IQueryable<T>> GetQueryableAsync(Expression<Func<T, bool>>? criteria = null);
    }

    public interface ICharacterRepository : IRepository<Character, int>
    {
        Task AddRangeAsync(IEnumerable<Character> characters);

        Task DeleteRangeByCriteriaAsync(Expression<Func<Character, bool>>? criteria = null);
    }

    public interface  ICharacterRepositoryDecorator : ICharacterRepository
    {
        Task InvalidateCache();
    }
}
