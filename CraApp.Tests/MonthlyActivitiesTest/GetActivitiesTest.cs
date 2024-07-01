
namespace CraApp.Tests.MonthlyActivitiesTest;

public class GetMonthlyActivitesTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private  APIResponse _APIresponse;

    public GetMonthlyActivitesTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void Sould_Return_Activities()
    {

        var response = await _client.GetAsync("/api/monthlyActivities");
        
        //_response = result.Adapt<APIResponse>();
        
        _APIresponse = await response.Content.ReadFromJsonAsync<APIResponse>();
        
        Assert.NotNull(_APIresponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(_APIresponse.Result);
    }

}
