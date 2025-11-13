using System.Net.Http.Json;
using BrainBay.Core.Entities;
using BrainBay.Core.Repositories;
using BrainBay.Model.Responses;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BrainBay.Console.BackgroundWorker
{
    public interface ICharacterSyncService
    {
        Task ExecuteOnceAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Background service that periodically synchronizes Rick and Morty characters
    /// and refreshes cached data.
    /// </summary>
    public class CharacterSyncService : BackgroundService, ICharacterSyncService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<CharacterSyncService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;
        private readonly TimeSpan _refreshInterval;
        private readonly string _cacheKey;

        public CharacterSyncService(
            IHttpClientFactory clientFactory,
            IDistributedCache cache,
            ICharacterRepository characterRepository,
            IConfiguration config,
            ILogger<CharacterSyncService> logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _refreshInterval = TimeSpan.FromHours(ParseRefreshInterval(config["RefreshIntervalInHours"]));
            _cacheKey = config.GetValue<string>("CharacterCacheKey") ?? "characters_cache_key";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Rick and Morty character synchronization...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExecuteOnceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during character synchronization.");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }
        }

        /// <summary>
        /// Performs a single sync operation.
        /// </summary>
        public async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var characters = await FetchAllCharactersAsync(cancellationToken);
            if (characters.Count == 0)
            {
                _logger.LogWarning("No characters retrieved from the API.");
                return;
            }

            await _characterRepository.DeleteRangeByCriteriaAsync();
            await _characterRepository.AddRangeAsync(
                characters.Select(ch => new Character(
                    ch.Name,
                    ch.Status,
                    ch.Species,
                    ch.Type,
                    ch.Gender,
                    ch.Origin != null ? new Core.ValueTypes.Origin(ch.Origin.Name, ch.Origin.Url) : null,
                    ch.Location != null ? new Core.ValueTypes.Location(ch.Location.Name, ch.Location.Url) : null,
                    ch.Image,
                    string.Join(",", ch.Episode))));

            await _characterRepository.SaveChangesAsync();
            await InvalidateCacheAsync();

            _logger.LogInformation("{Count} characters successfully synced and cached cleared.", characters.Count);
        }

        /// <summary>
        /// Fetches all paginated characters from the Rick & Morty API.
        /// </summary>
        private async Task<List<CharacterResponse>> FetchAllCharactersAsync(CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient("RickAndMorty");
            var allCharacters = new List<CharacterResponse>();
            string? nextUrl = string.Empty;

            var initialResponse = await client.GetFromJsonAsync<RickAndMortyResponse>("", cancellationToken);
            if (initialResponse?.Results == null) return allCharacters;

            allCharacters.AddRange(initialResponse.Results);
            nextUrl = initialResponse.Info.Next;

            while (!string.IsNullOrEmpty(nextUrl))
            {
                var response = await client.GetFromJsonAsync<RickAndMortyResponse>(nextUrl, cancellationToken);
                if (response?.Results == null) break;

                allCharacters.AddRange(response.Results);
                nextUrl = response.Info.Next;

                await Task.Delay(100, cancellationToken);
            }

            return allCharacters;
        }

        private async Task InvalidateCacheAsync()
        {
            await _cache.RemoveAsync(_cacheKey);
            _logger.LogInformation("Character cache invalidated (key: {CacheKey}).", _cacheKey);
        }

        private static int ParseRefreshInterval(string? intervalStr)
        {
            if (int.TryParse(intervalStr, out var hours) && hours > 0)
                return hours;

            return 1; // default fallback
        }
    }
}
