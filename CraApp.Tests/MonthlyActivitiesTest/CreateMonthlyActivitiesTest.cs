
namespace CraApp.Tests.MonthlyActivitiesTest;



public class CreateMonthlyActivitiesTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private APIResponse _APIResponse;
    MonthlyActivitiesDTO _monthlyActivitiesDTO;

    public CreateMonthlyActivitiesTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _APIResponse = new APIResponse();
        _monthlyActivitiesDTO = new MonthlyActivitiesDTO
        {
            Year = 2024,
            Month = 2,
            Activities = new List<ActivityDTO>
    {
        new ActivityDTO
        {
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = 1,
            Project = Project.Formation.ToString(),
        },
        new ActivityDTO
        {
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            Day = 2,
            Project = Project.MyTaraji.ToString(),

        }

    }
        };
    }

    public async Task<MonthlyActivitiesDTO> RequestHandler(MonthlyActivitiesDTO _monthlyactivities)
    {


        var content = JsonContent.Create(_monthlyactivities);

        var response = await _client.PostAsync("/api/monthlyActivities", content);

        var result = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);
        if (_APIResponse.Result is not null)
        {
            var jsonElement = (JsonElement)_APIResponse.Result;
            var createdActivity = JsonSerializer.Deserialize<MonthlyActivitiesDTO>(jsonElement.GetRawText(), options);
            return createdActivity;
        }
        return new MonthlyActivitiesDTO();


    }

    [Fact]
    public async void Should_Save_Monthly_Activities()
    {
       

        var _createdMonthlyActivities = await RequestHandler(_monthlyActivitiesDTO);
       
        Assert.Equal(HttpStatusCode.Created, _APIResponse.StatusCode);
        Assert.NotNull(_APIResponse.Result);
        Assert.True(_APIResponse.IsSuccess);

        Assert.Equal(_createdMonthlyActivities.Year, _monthlyActivitiesDTO.Year);
        Assert.Equal(_createdMonthlyActivities.Month, _monthlyActivitiesDTO.Month);


    }
    [Fact]
    public async void Should_Not_Save_If_Activities_Days_Dont_Match_Month_Days()
    {
        _monthlyActivitiesDTO.Activities.Add(new ActivityDTO
        {
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = -4,
            MonthlyActivitiesId = 1
        });

        var _createdMonthlyActivities = await RequestHandler(_monthlyActivitiesDTO);

        Assert.True(!_APIResponse.IsSuccess);
        Assert.Null(_APIResponse.Result);
        Assert.NotNull(_APIResponse.ErrorsMessages);
        Assert.Equal("Day cannot exceded month max number of days or be less than zero. ", _APIResponse.ErrorsMessages.First());
    }
}
