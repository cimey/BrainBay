using System.Net;
using System.Net.Http.Json;
using BrainBay.Core.Entities;
using BrainBay.Infrastructure.DatabaseContext;
using BrainBay.IntegrationTests.Infrastructure;
using BrainBay.Model.Responses;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace BrainBay.IntegrationTests.Tests
{
    public class CharacterControllerTests : TestBase, IAsyncLifetime
    {
        private bool _firstCall = true;
        public CharacterControllerTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact, TestPriority(1)]
        public async Task GetCharacters_ShouldReturnOkAndListOfCharacters()
        {
            // Act
            var response = await _client.GetAsync("/api/characters?skip=0&take=2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var characters = await response.Content.ReadFromJsonAsync<List<Character>>();
            characters.Should().NotBeNull();
            characters!.Count.Should().Be(2);
            characters!.All(c => c.Status == "Alive").Should().BeTrue();
            response.Headers.First(x => x.Key == "from-database").Should().NotBeNull();
            response.Headers.First(x => x.Key == "from-database").Value.Should().Equal("true");


            response = await _client.GetAsync("/api/characters?skip=0&take=2");
            response.Headers.First(x => x.Key == "from-database").Should().NotBeNull();
            response.Headers.First(x => x.Key == "from-database").Value.Should().Equal("false");
        }

        [Fact, TestPriority(2)]
        public async Task CreateCharacter_ShouldReturnCreated()
        {
            // Arrange
            var newCharacter = new
            {
                name = "Summer Smith",
                status = "Alive",
                species = "Human",
                type = "",
                gender = "Female",
                origin = new { name = "Earth", url = "https://rickandmortyapi.com/api/location/1" },
                location = new { name = "Earth", url = "https://rickandmortyapi.com/api/location/1" },
                image = "https://rickandmortyapi.com/api/character/avatar/3.jpeg",
                episode = new string[] { "https://rickandmortyapi.com/api/episode/1", "https://rickandmortyapi.com/api/episode/2" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/characters", newCharacter);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<CharacterDto>();
            created.Should().NotBeNull();
            created!.Name.Should().Be("Summer Smith");
            created.Status.Should().Be("Alive");
        }


        [Fact, TestPriority(3)]
        public async Task GetCharacters_ShouldIncludeCreatedCharacter()
        {
            // Act
            var response = await _client.GetAsync("/api/characters?skip=0&take=100");

            // Assert
            response.EnsureSuccessStatusCode();

            var characters = await response.Content.ReadFromJsonAsync<List<CharacterDto>>();
            characters.Should().NotBeNull();
            characters!.Any(c => c.Name == "Summer Smith").Should().BeTrue();

            //cache invalidated after creation and it should return from db
            response.Headers.First(x => x.Key == "from-database").Should().NotBeNull();
            response.Headers.First(x => x.Key == "from-database").Value.Should().Equal("true");
        }

        [Fact, TestPriority(4)]
        public async Task GetById_Should_Return()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/characters?skip=0&take=1");

            // Assert
            response.EnsureSuccessStatusCode();

            var characters = await response.Content.ReadFromJsonAsync<List<CharacterDto>>();
            var character = characters!.First();
            var getByIdResponse = await _client.GetAsync($"/api/characters/{character.Id}");

            var characterById = await getByIdResponse.Content.ReadFromJsonAsync<CharacterDto>();
            characterById.Should().NotBeNull();
            getByIdResponse.Headers.First(x => x.Key == "from-database").Should().NotBeNull();
            getByIdResponse.Headers.First(x => x.Key == "from-database").Value.Should().Equal("false");
        }

        public async Task InitializeAsync()
        {

            using var scope = _factory.Services.CreateScope();
            // Arrange - seed test data
            var db = scope.ServiceProvider.GetRequiredService<BrainBayDbContext>();
            db.Characters.Add(new Character("Rick Sanchez", "Alive", "Human", "", "Male", null, null, "https://image.jpg", "1,2,3"));
            db.Characters.Add(new Character("Morty Smith", "Alive", "Human", "", "Male", null, null, "https://image2.jpg", "1,2"));
            db.SaveChanges();

            if (_firstCall)
            {
                var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                cache.Remove(_factory.Configuration["CharacterCacheKey"] ?? "_characters");
            }
            _firstCall = false;
            await Task.CompletedTask;
        }
    }
}
