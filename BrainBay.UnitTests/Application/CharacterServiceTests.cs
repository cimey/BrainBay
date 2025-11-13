using System.Linq.Expressions;
using AutoMapper;
using BrainBay.Application.Features.Character;
using BrainBay.Core.Entities;
using BrainBay.Core.Repositories;
using BrainBay.Model.Inputs;
using BrainBay.Model.Requests;
using BrainBay.Model.Responses;
using Moq;
namespace BrainBay.UnitTests.Application
{
    namespace BrainBay.UnitTests.Application.Features
    {
        public class CharacterServiceTests
        {
            private readonly Mock<ICharacterRepositoryDecorator> _mockRepo;
            private readonly IMapper _mapper;
            private readonly CharacterService _service;

            public CharacterServiceTests()
            {
                _mockRepo = new Mock<ICharacterRepositoryDecorator>();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Character, CharacterDto>();
                });
                _mapper = config.CreateMapper();

                _service = new CharacterService(_mockRepo.Object, _mapper);
            }

            [Fact]
            public async Task GetCharactersAsync_ReturnsMappedDtos()
            {
                // Arrange
                var characters = new List<Character>
            {
                new("Rick", "Alive", "Human", "", "Male", null, null, "https://rick.jpg", "1,2"),
                new("Morty", "Alive", "Human", "", "Male", null, null, "https://morty.jpg", "3,4")
            };
                _mockRepo.Setup(r => r.GetQueryableAsync(It.IsAny<Expression<Func<Character, bool>>>()))
                    .ReturnsAsync(characters.AsQueryable());

                var request = new GetCharactersRequest { Skip = 0, PageSize = 10 };

                // Act
                var result = await _service.GetCharactersAsync(request);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, c => Assert.Equal("Alive", c.Status));
            }

            [Fact]
            public async Task GetCharacterAsync_ReturnsMappedCharacter()
            {
                // Arrange
                var character = new Character("Rick", "Alive", "Human", "", "Male", null, null, "https://rick.jpg", "1,2");
                _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(character);

                // Act
                var result = await _service.GetCharacterAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Rick", result.Name);
            }

            [Fact]
            public async Task CreateCharacterAsync_AddsCharacter_AndInvalidatesCache()
            {
                // Arrange
                var input = new CreateCharacterInput
                {
                    Name = "Summer",
                    Status = "Alive",
                    Species = "Human",
                    Gender = "Female",
                    Image = "https://summer.jpg",
                    Episode = new[] { "1", "2" }.ToList()
                };

                var createdEntity = new Character("Summer", "Alive", "Human", "", "Female", null, null, "https://summer.jpg", "1,2");

                _mockRepo.Setup(r => r.AddAsync(It.IsAny<Character>()))
                         .ReturnsAsync(createdEntity);

                _mockRepo.Setup(r => r.InvalidateCache())
                         .Returns(Task.CompletedTask)
                         .Verifiable();

                // Act
                var result = await _service.CreateCharacterAsync(input);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Summer", result.Name);
                _mockRepo.Verify(r => r.AddAsync(It.IsAny<Character>()), Times.Once);
                _mockRepo.Verify(r => r.InvalidateCache(), Times.Once);
            }
        }
    }

}
