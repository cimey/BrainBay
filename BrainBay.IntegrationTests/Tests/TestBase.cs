namespace BrainBay.IntegrationTests.Tests
{
    public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory _factory;
        public TestBase(CustomWebApplicationFactory factory)
        { 
            _factory = factory;
            _client = _factory.CreateClient();
        }
    }
}
