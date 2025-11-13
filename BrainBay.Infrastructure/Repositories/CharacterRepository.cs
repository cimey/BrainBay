using System.Linq.Expressions;
using BrainBay.Core.Entities;
using BrainBay.Core.Repositories;
using BrainBay.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace BrainBay.Infrastructure.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly BrainBayDbContext _context;
        private readonly DbSet<Character> _dbSet;
        public CharacterRepository(BrainBayDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Character>();
        }

        public async Task<Character> AddAsync(Character entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Character> characters)
        {
            await _dbSet.AddRangeAsync(characters);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
        }

        public async Task DeleteRangeByCriteriaAsync(Expression<Func<Character, bool>>? criteria = null)
        {
            var entities = criteria == null ? await _dbSet.ToListAsync() : await _dbSet.Where(criteria).ToListAsync();
            _dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<Character>> FilterAsync(Expression<Func<Character, bool>>? criteria)
        {
            if (criteria == null)
            {
                return await _dbSet.ToListAsync();
            }
            return await _dbSet.Where(criteria).ToListAsync();
        }

        public async Task<Character?> GetByIdAsync(int id)
        {

            return await _dbSet.FindAsync(id);
        }

        public async Task<IQueryable<Character>> GetQueryableAsync(Expression<Func<Character, bool>>? criteria = null)
        {
            var entities = criteria == null ? _dbSet : _dbSet.Where(criteria);

            return await Task.FromResult(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Character entity)
        {
            _dbSet.Update(entity);

            await Task.CompletedTask;
        }
    }
}
