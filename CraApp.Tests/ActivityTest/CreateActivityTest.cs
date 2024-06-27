using CraApp.Tests.Util;

namespace CraApp.Tests.ActivityTest;

public class CreateActivityTest
{
    //private readonly Mock<CreateActivity> _createActivityMock;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string url = "/api/activity";

    public CreateActivityTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
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

    var createdActivity = await Helper.Post(_activity, url, _client);
    // Assert the values
    Assert.Equal(HttpStatusCode.Created, Helper._APIResponse.StatusCode);
    Assert.NotNull(Helper._APIResponse.Result);
    Assert.True(Helper._APIResponse.IsSuccess);

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

        var result = await Helper.Post(_activity, url, _client);

        Assert.Equal(HttpStatusCode.BadRequest, Helper._APIResponse.StatusCode);
        Assert.Null(Helper._APIResponse.Result);
        Assert.NotEmpty(Helper._APIResponse.ErrorsMessages);
        Assert.True(!Helper._APIResponse.IsSuccess);
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

        var result = await Helper.Post(_activity, url, _client);

        Assert.Equal(HttpStatusCode.BadRequest, Helper._APIResponse.StatusCode);
        Assert.Null(Helper._APIResponse.Result);
        Assert.NotEmpty(Helper._APIResponse.ErrorsMessages);
        Assert.True(!Helper._APIResponse.IsSuccess);

    }

   
}
