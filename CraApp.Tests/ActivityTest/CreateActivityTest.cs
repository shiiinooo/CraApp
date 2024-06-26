namespace CraApp.Tests.ActivityTest;

public class CreateActivityTest
{
    //private readonly Mock<CreateActivity> _createActivityMock;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;


    public CreateActivityTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _APIResponse = new APIResponse();
    }

    public async Task<ActivityDTO> RequestHandler(ActivityDTO _activity)
    {


        var content = JsonContent.Create(_activity);

        var response = await _client.PostAsync("/api/activity", content);

        var result = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);
        if (_APIResponse.Result is not null)
        {
            var jsonElement = (JsonElement)_APIResponse.Result;
            var createdActivity = JsonSerializer.Deserialize<ActivityDTO>(jsonElement.GetRawText(), options);
            return createdActivity;
        }
        return new ActivityDTO();
       
    }
    [Fact]
    public async void Should_Save_Activity()
    {
        ActivityDTO _activity = new ActivityDTO
        {
            Project = Project.MyTaraji.ToString(),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = 2,
            MonthlyActivitiesId = 1
            
        };

        var createdActivity = await RequestHandler(_activity);
        // Assert the values
        Assert.Equal(HttpStatusCode.Created, _APIResponse.StatusCode);
        Assert.NotNull(_APIResponse.Result);
        Assert.True(_APIResponse.IsSuccess);

        Assert.Equal(_activity.StartTime, createdActivity.StartTime);
        Assert.Equal(_activity.EndTime, createdActivity.EndTime);
        Assert.Equal(_activity.Project, createdActivity.Project);

    }

    [Fact]
    public async void Should_Throw_Exception_For_StartTime_Greater_Than_EndTime()
    {
        ActivityDTO _activity = new ActivityDTO
        {
            Project = Project.MyTaraji.ToString(),
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            Day = 2,
            MonthlyActivitiesId = 1
        };

        var result = await RequestHandler(_activity);

        Assert.Equal(HttpStatusCode.BadRequest, _APIResponse.StatusCode);
        Assert.Null(_APIResponse.Result);
        Assert.NotEmpty(_APIResponse.ErrorsMessages);
        Assert.True(!_APIResponse.IsSuccess);
    }

    [Fact]
    public async void Should_Not_Save_Activity_if_MaxHours_exceded_Limits()
    {

        ActivityDTO _activity = new ActivityDTO
        {
            Project = Project.MyTaraji.ToString(),
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            Day = 2,
            MonthlyActivitiesId = 1
        };

        var result = await RequestHandler(_activity);

        Assert.Equal(HttpStatusCode.BadRequest, _APIResponse.StatusCode);
        Assert.Null(_APIResponse.Result);
        Assert.NotEmpty(_APIResponse.ErrorsMessages);
        Assert.True(!_APIResponse.IsSuccess);

    }

   
}
