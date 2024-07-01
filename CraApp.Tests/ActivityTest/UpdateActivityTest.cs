

using CraApp.Tests.Util;

namespace CraApp.Tests.ActivityTest;

public class UpdateActivityTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;


    public UpdateActivityTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _APIResponse = new APIResponse();
    }

    [Fact]
    public async  void Should_Update_Activity()
    {
        MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);
        ActivityDTO ActivityDTO = monthlyActivitiesDTO.Activities.First();
        
        ActivityDTO.StartTime = new TimeSpan(09, 30, 00);
        ActivityDTO.EndTime = new TimeSpan(15, 30, 00);
        ActivityDTO.Project = Project.Formation.ToString();

        var content = JsonContent.Create(ActivityDTO);
        var response = await _client.PutAsync("/api/activity", content);

        var result = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);

        var jsonElement = (JsonElement)_APIResponse.Result;
        var updatedActivity = JsonSerializer.Deserialize<ActivityDTO>(jsonElement.GetRawText(), options);

        Assert.NotNull(_APIResponse);
        Assert.Empty(_APIResponse.ErrorsMessages);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(ActivityDTO.StartTime, updatedActivity.StartTime);
        Assert.Equal(ActivityDTO.EndTime, updatedActivity.EndTime);
        Assert.Equal(ActivityDTO.Day, updatedActivity.Day);
        Assert.Equal(ActivityDTO.Project, updatedActivity.Project);
        Assert.Equal(ActivityDTO.MonthlyActivitiesId, updatedActivity.MonthlyActivitiesId);

        await Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id);
    }

}
