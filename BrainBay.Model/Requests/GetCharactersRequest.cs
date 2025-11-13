namespace BrainBay.Model.Requests
{
    public class GetCharactersRequest
    {
        public int Skip { get; set; } = 0;

        public int PageSize { get; set; } = 20;
    }
}
