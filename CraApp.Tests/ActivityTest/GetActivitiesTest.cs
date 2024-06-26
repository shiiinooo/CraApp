﻿
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
        Assert.True(_APIresponse.IsSuccess);    
        Assert.Equal(HttpStatusCode.OK, _APIresponse.StatusCode);
        Assert.NotNull(_APIresponse.Result);
    }
}