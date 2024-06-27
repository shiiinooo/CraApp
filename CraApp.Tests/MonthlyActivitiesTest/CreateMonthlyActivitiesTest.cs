
using System;
using CraApp.Tests.Util;

namespace CraApp.Tests.MonthlyActivitiesTest;



public class CreateMonthlyActivitiesTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private MonthlyActivitiesDTO _monthlyActivitiesDTO;
    private readonly string url = "/api/monthlyActivities";

    public CreateMonthlyActivitiesTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
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

   

    [Fact]
    public async void Should_Save_Monthly_Activities()
    {

        
        var _createdMonthlyActivities = await Helper.Post(_monthlyActivitiesDTO, url,_client);
       
        Assert.Equal(HttpStatusCode.NoContent, Helper._APIResponse.StatusCode);
        Assert.NotNull(Helper._APIResponse.Result);
        Assert.True(Helper._APIResponse.IsSuccess);

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

        var _createdMonthlyActivities = await Helper.Post(_monthlyActivitiesDTO, url, _client);

        Assert.True(!Helper._APIResponse.IsSuccess);
        Assert.Null(Helper._APIResponse.Result);
        Assert.NotNull(Helper._APIResponse.ErrorsMessages);
        Assert.Equal("Day cannot exceded month max number of days or be less than zero. ", Helper._APIResponse.ErrorsMessages.First());
    }
}
