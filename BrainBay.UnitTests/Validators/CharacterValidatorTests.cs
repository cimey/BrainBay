using BrainBay.API.Validators;
using BrainBay.Model.Inputs;
using BrainBay.Model.Responses;
using FluentValidation.TestHelper;

namespace BrainBay.UnitTests.Validators
{
    public class CharacterValidatorTests
    {
        private readonly CharacterValidator _validator = new();

        [Fact]
        public void Should_HaveError_When_Name_IsEmpty()
        {
            var model = new CreateCharacterInput { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void Should_HaveError_When_Status_IsInvalid()
        {
            var model = new CreateCharacterInput { Name = "Rick", Status = "Undefined" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(c => c.Status);
        }

        [Fact]
        public void Should_NotHaveError_When_Status_IsValid()
        {
            var model = new CreateCharacterInput { Name = "Rick", Status = "Alive" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(c => c.Status);
        }

        [Fact]
        public void Should_HaveError_When_ImageUrl_IsInvalid()
        {
            var model = new CreateCharacterInput { Name = "Rick", Status = "Alive", Image = "invalid-url" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(c => c.Image);
        }

        [Fact]
        public void Should_NotHaveError_When_ImageUrl_IsValid()
        {
            var model = new CreateCharacterInput { Name = "Rick", Status = "Alive", Image = "https://example.com/image.png" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(c => c.Image);
        }

        [Fact]
        public void Should_HaveError_When_Episodes_AreInvalidUrls()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Episode = new List<string> { "invalid-url", "https://valid.com" }
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(c => c.Episode);
        }

        [Fact]
        public void Should_NotHaveError_When_Episodes_AreValidUrls()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Episode = new List<string> { "https://api.example.com/1", "https://api.example.com/2" }
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(c => c.Episode);
        }

        [Fact]
        public void Should_HaveError_When_Location_IsInvalid()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Location = new LocationResponse ("", "invalid-url" )
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("Location.Name");
            result.ShouldHaveValidationErrorFor("Location.Url");
        }

        [Fact]
        public void Should_NotHaveError_When_Location_IsValid()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Location = new LocationResponse
                ("Earth", "https://example.com/location/1")
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor("Location.Name");
            result.ShouldNotHaveValidationErrorFor("Location.Url");
        }

        [Fact]
        public void Should_HaveError_When_Origin_IsInvalid()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Origin = new OriginResponse ( "",  "ftp://wrong.com" )
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("Origin.Name");
            result.ShouldHaveValidationErrorFor("Origin.Url");
        }

        [Fact]
        public void Should_NotHaveError_When_Origin_IsValid()
        {
            var model = new CreateCharacterInput
            {
                Name = "Rick",
                Status = "Alive",
                Origin = new OriginResponse
                ("Earth (C-137)",
                    "https://api.example.com/origin/1"
                )
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor("Origin.Name");
            result.ShouldNotHaveValidationErrorFor("Origin.Url");
        }
    }
}
