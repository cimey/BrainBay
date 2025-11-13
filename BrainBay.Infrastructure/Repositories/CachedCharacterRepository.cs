using System.Linq.Expressions;
using System.Text.Json;
using BrainBay.Core.Entities;
using BrainBay.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace BrainBay.Infrastructure.Repositories
{
    public class CachedCharacterRepository : ICharacterRepositoryDecorator
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _cacheDuration;
        private readonly string _cacheKey;
        private readonly ICharacterRepository _characterRepository;
        private readonly IConfiguration _config;


        public CachedCharacterRepository(ICharacterRepository characterRepository, IDistributedCache cache, IConfiguration config)
        {
            _cache = cache;
            _characterRepository = characterRepository;
            _config = config;
            _cacheKey = _config["CharacterCacheKey"] ?? "characters_cache_key";

            _cacheDuration = TimeSpan.FromMinutes(int.TryParse(_config["CacheDurationMinutes"], out int cacheDuration) ? cacheDuration : 5);
        }

        public async Task<IEnumerable<Character>> FilterAsync(Expression<Func<Character, bool>>? criteria)
        {
            var characters = await GetCachedCharactersAsync();

            if (criteria != null)
                return characters.AsQueryable().Where(criteria);

            return characters;
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            var characters = await GetCachedCharactersAsync();
            return characters.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IQueryable<Character>> GetQueryableAsync(Expression<Func<Character, bool>>? criteria = null)
        {
            var characters = await GetCachedCharactersAsync();

            var queryable = characters.AsQueryable();
            if (criteria != null)
                queryable = queryable.Where(criteria);

            return queryable;
        }

        public async Task AddRangeAsync(IEnumerable<Character> characters)
        {
            await _characterRepository.AddRangeAsync(characters);
        }

        public async Task DeleteRangeByCriteriaAsync(Expression<Func<Character, bool>>? criteria = null)
        {
            await _characterRepository.DeleteRangeByCriteriaAsync(criteria);
        }

        public async Task<Character> AddAsync(Character entity)
        {
            return await _characterRepository.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _characterRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Character entity)
        {
            await _characterRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _characterRepository.DeleteAsync(id);
        }

        public async Task InvalidateCache()
        {
            _cache.Remove(_cacheKey);

            await Task.CompletedTask;
        }

        private async Task<List<Character>> GetCachedCharactersAsync()
        {
            // Try get from Redis
            var cachedData = await _cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCharacters = JsonSerializer.Deserialize<List<Character>>(cachedData) ?? new List<Character>();
                cachedCharacters.ForEach(c => c.FromCache = true);
                return cachedCharacters;
            }

            // If not found, query from DB
            var entities = (await _characterRepository.GetQueryableAsync()).ToList();
            entities.ForEach(c => c.FromCache = false);

            // Store in Redis
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            };
            var json = JsonSerializer.Serialize(entities);
            await _cache.SetStringAsync(_cacheKey, json, options);

            return entities;
        }
    }
}
