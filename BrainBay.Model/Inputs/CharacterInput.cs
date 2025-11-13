using BrainBay.Model.Responses;

namespace BrainBay.Model.Inputs
{
    public class CreateCharacterInput
    {
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public string Species { get; set; } = "";
        public string Type { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Image { get; set; } = "";
        public OriginResponse? Origin { get; set; }

        public LocationResponse? Location { get; set; }

        public List<string> Episode { get; set; } = new();
    }
}
