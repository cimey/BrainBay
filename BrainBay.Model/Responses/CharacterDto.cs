using System.Text.Json.Serialization;

namespace BrainBay.Model.Responses
{
    public class CharacterDto : CharacterResponse, ICachedResult
    {
        [JsonIgnore]
        public bool FromCache { get; set; }
    }

    public interface ICachedResult
    {
        public bool FromCache { get; set; }
    }
}
