using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BrainBay.Core.ValueTypes;

namespace BrainBay.Core.Entities
{
    public class Character
    {
        private Character() { }

        /// <summary>
        /// For creating Character with Id (used internally)
        /// </summary>
        [JsonConstructor]
        private Character(int id, 
            string name,
            string status,
            string species,
            string type,
            string gender,
            Origin? origin,
            Location? location,
            string image,
            string episodes) : this(name,status, species, type, gender, origin, location, image, episodes) { Id = id;  }

        public Character(
            string name,
            string status,
            string species,
            string type,
            string gender,
            Origin? origin,
            Location? location,
            string image,
            string episodes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required", nameof(status));

            if (name.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));

            if (status.Length > 50)
                throw new ArgumentException("Status cannot exceed 50 characters", nameof(status));

            if (species.Length > 50)
                throw new ArgumentException("Species cannot exceed 50 characters", nameof(species));

            if (type.Length > 50)
                throw new ArgumentException("Type cannot exceed 50 characters", nameof(type));

            if (gender.Length > 20)
                throw new ArgumentException("Gender cannot exceed 20 characters", nameof(gender));

            if (image.Length > 300)
                throw new ArgumentException("Image URL cannot exceed 300 characters", nameof(image));
            
            if (!string.IsNullOrWhiteSpace(image) &&
                !Uri.TryCreate(image, UriKind.Absolute, out var _))
                throw new ArgumentException("Image must be a valid URL", nameof(image));

            // Validate nested objects (if not null)
            if (origin != null)
            {
                if (string.IsNullOrWhiteSpace(origin.Name))
                    throw new ArgumentException("Origin name is required", nameof(origin));
                if (!string.IsNullOrWhiteSpace(origin.Url) && !Uri.IsWellFormedUriString(origin.Url, UriKind.Absolute))
                    throw new ArgumentException("Origin URL must be valid", nameof(origin));
            }

            if (location != null)
            {
                if (string.IsNullOrWhiteSpace(location.Name))
                    throw new ArgumentException("Location name is required", nameof(location));
                if (!string.IsNullOrWhiteSpace(location.Url) && !Uri.IsWellFormedUriString(location.Url, UriKind.Absolute))
                    throw new ArgumentException("Location URL must be valid", nameof(location));
            }

            if (!string.IsNullOrWhiteSpace(episodes))
            {
                var episodeList = episodes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var ep in episodeList)
                {
                    if (!Uri.IsWellFormedUriString(ep, UriKind.Absolute))
                        throw new ArgumentException("Episode URL must be valid", nameof(episodes));
                }
            }

            Name = name;
            Status = status;
            Species = species;
            Type = type;
            Gender = gender;
            Origin = origin;
            Location = location;
            Image = image;
            Episodes = episodes;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        [Key]
        public int Id { get; protected set; }

        [Required]
        public string Name { get; private set; } = string.Empty;

        public string Status { get; private set; } = string.Empty;
        public string Species { get; private set; } = string.Empty;
        public string Type { get; private set; } = string.Empty;
        public string Gender { get; private set; } = string.Empty;

        public Origin? Origin { get; private set; }

        public Location? Location { get; private set; }

        public string Image { get; private set; } = string.Empty;

        public string Episodes { get; private set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; private set; }

        [NotMapped]
        public bool FromCache { get; set; }
    }

}
