﻿
using CraApp.Tests.Util;

namespace CraApp.Tests.MonthlyActivitiesTest;

public class DeleteMonthlyActitivities
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;

    public DeleteMonthlyActitivities()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _APIResponse = new APIResponse();
    }

    [Fact]
    public async void Should_Delete_MonthlyActivities()
    {
        MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

        int Id = monthlyActivitiesDTO.Id;

        var response = await _client.DeleteAsync($"/api/monthlyActivities/{Id}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);
        Assert.Equal(HttpStatusCode.NoContent, _APIResponse.StatusCode);
        Assert.True(_APIResponse.IsSuccess);
    }

    [Fact]
    public async void Should_Throw_KeyNotFoundException_Excepttion()
    {

        int Id = 99999999;
        var response = await _client.DeleteAsync($"/api/monthlyActivities/{Id}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);
        Assert.Equal(HttpStatusCode.NotFound, _APIResponse.StatusCode);
        Assert.True(!_APIResponse.IsSuccess);
        Assert.Equal("Key Not Found ", _APIResponse.ErrorsMessages.First());
    }
}
