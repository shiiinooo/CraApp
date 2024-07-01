
namespace CraApp.Tests.ActivityTest;

public class GetActivitiesTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private  APIResponse _APIresponse;

    public GetActivitiesTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void Sould_Return_Activities()
    {

        var result = await _client.GetAsync("/api/activity");
        
        //_response = result.Adapt<APIResponse>();
        
        _APIresponse = await result.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(_APIresponse);
        Assert.NotNull(_APIresponse.Result);
        Assert.Empty(_APIresponse.ErrorsMessages); // Ensure there are no error messages
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}
