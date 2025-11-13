namespace BrainBay.Model.Responses
{
    public class DataInfo
    {
        public int Count { get; set; }
        public int Pages { get; set; }
        public string? Next { get; set; }
        public string? Prev { get; set; }
    }
    public class CharacterResponse
    {
        public int Id { get; set; }
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

    public record OriginResponse(string Name, string Url);

    public record LocationResponse(string Name, string Url);


    public class RickAndMortyResponse
    {
        public DataInfo Info { get; set; }
        public List<CharacterResponse> Results { get; set; } = new();
    }
}
