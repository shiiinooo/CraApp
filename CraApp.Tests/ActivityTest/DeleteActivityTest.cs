

using System.Diagnostics;
using CraApp.Model;
using CraApp.Repository.IRepository;
using CraApp.Tests.Util;

namespace CraApp.Tests.ActivityTest;

public class DeleteActivityTest
{

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;
    private LoginRequestDTO _adminLoginRequestDTO;

    public DeleteActivityTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _APIResponse = new APIResponse();
        _adminLoginRequestDTO = new LoginRequestDTO
        {
            UserName = "shiinoo",
            Password = "Password123#"
        };
    }

    [Fact]
    public async void Should_Delete_Activity()
    {
       
        MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

        int ActivityId = monthlyActivitiesDTO.Activities.FirstOrDefault().Id;
        var response = await _client.DeleteAsync($"/api/activity/{ActivityId}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result,options);
        await Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);
    }

    [Fact]
    public async void Should_Throw_KeyNotFoundException_Excepttion()
    {

        int ActivityId = 99999999;
        var response = await _client.DeleteAsync($"/api/activity/{ActivityId}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);;
        Assert.Equal("Key Not Found ", _APIResponse.ErrorsMessages.First());
    }
}
