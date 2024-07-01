
using CraApp.Tests.Util;

namespace CraApp.Tests.MonthlyActivitiesTest;

public class DeleteMonthlyActitivities
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;
    private LoginRequestDTO _adminLoginRequestDTO;
    public DeleteMonthlyActitivities()
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
    public async void Should_Delete_MonthlyActivities()
    {
        MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

        int Id = monthlyActivitiesDTO.Id;

        var response = await _client.DeleteAsync($"/api/monthlyActivities/{Id}");

        var result = await response.Content.ReadAsStringAsync();
       

      
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);
    }

    [Fact]
    public async void Should_Throw_KeyNotFoundException_Excepttion()
    {

        int Id = 99999999;
        var response = await _client.DeleteAsync($"/api/monthlyActivities/{Id}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Key Not Found", _APIResponse.ErrorsMessages.First());
    }
}
