using BrainBay.Core.Entities;
using BrainBay.Core.ValueTypes;

namespace BrainBay.UnitTests.Core
{
    public class CharacterEntityTests
    {
        private static readonly Origin ValidOrigin = new("Earth", "https://example.com/origin/1");
        private static readonly Location ValidLocation = new("Citadel", "https://example.com/location/1");

        [Fact]
        public void Should_CreateCharacter_When_AllFieldsValid()
        {
            // Arrange
            var episodes = "https://example.com/1,https://example.com/2";

            // Act
            var character = new Character(
                name: "Rick Sanchez",
                status: "Alive",
                species: "Human",
                type: "",
                gender: "Male",
                origin: ValidOrigin,
                location: ValidLocation,
                image: "https://example.com/image.jpg",
                episodes: episodes);

            // Assert
            Assert.Equal("Rick Sanchez", character.Name);
            Assert.Equal("Alive", character.Status);
            Assert.False(character.FromCache);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Throw_When_NameIsMissing(string? invalidName)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Character(
                invalidName!,
                "Alive",
                "Human",
                "",
                "Male",
                ValidOrigin,
                ValidLocation,
                "https://example.com/image.jpg",
                "https://example.com/ep1"));

            Assert.Contains("Name is required", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Throw_When_StatusIsMissing(string? invalidStatus)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Character(
                "Rick",
                invalidStatus!,
                "Human",
                "",
                "Male",
                ValidOrigin,
                ValidLocation,
                "https://example.com/image.jpg",
                "https://example.com/ep1"));

            Assert.Contains("Status is required", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_NameTooLong()
        {
            var longName = new string('A', 101);

            var ex = Assert.Throws<ArgumentException>(() => new Character(
                longName,
                "Alive",
                "Human",
                "",
                "Male",
                ValidOrigin,
                ValidLocation,
                "https://example.com/image.jpg",
                "https://example.com/ep1"));

            Assert.Contains("Name cannot exceed 100 characters", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_InvalidImageUrl()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Character(
                "Rick",
                "Alive",
                "Human",
                "",
                "Male",
                ValidOrigin,
                ValidLocation,
                "invalid-url",
                "https://example.com/ep1"));

            Assert.Contains("Image must be a valid URL", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_OriginHasInvalidUrl()
        {
            var invalidOrigin = new Origin("Earth", "invalid-url");

            var ex = Assert.Throws<ArgumentException>(() => new Character(
                "Rick",
                "Alive",
                "Human",
                "",
                "Male",
                invalidOrigin,
                ValidLocation,
                "https://example.com/image.jpg",
                "https://example.com/ep1"));

            Assert.Contains("Origin URL must be valid", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_LocationHasInvalidUrl()
        {
            var invalidLocation = new Location("Citadel", "//url");

            var ex = Assert.Throws<ArgumentException>(() => new Character(
                "Rick",
                "Alive",
                "Human",
                "",
                "Male",
                ValidOrigin,
                invalidLocation,
                "https://example.com/image.jpg",
                "https://example.com/ep1"));

            Assert.Contains("Location URL must be valid", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_EpisodesContainInvalidUrls()
        {
            // Arrange
            var invalidEpisodes = "https://example.com/ep1,not-a-url,https://example.com/ep3";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Character(
                "Morty",
                "Alive",
                "Human",
                "",
                "Male",
                ValidOrigin,
                ValidLocation,
                "https://example.com/image.jpg",
                invalidEpisodes));

            Assert.Contains("Episode URL must be valid", ex.Message);
        }
    }
}
