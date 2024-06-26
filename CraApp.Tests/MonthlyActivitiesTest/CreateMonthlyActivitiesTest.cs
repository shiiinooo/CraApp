﻿
using System;
using CraApp.Tests.Util;

namespace CraApp.Tests.MonthlyActivitiesTest;



public class CreateMonthlyActivitiesTest
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private MonthlyActivitiesDTO _monthlyActivitiesDTO;
    private readonly string url = "/api/monthlyActivities";
    private LoginRequestDTO _adminLoginRequestDTO;

    public CreateMonthlyActivitiesTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _adminLoginRequestDTO = new LoginRequestDTO
        {
            UserName = "shiinoo",
            Password = "Password123#"
        };
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


        var monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);
       
        Assert.Equal(HttpStatusCode.Created, Helper._APIStatusCode);
        Assert.NotNull(Helper._APIResponse.Result);
        Assert.Equal(2023, monthlyActivitiesDTO.Year);
        Assert.Equal(12, monthlyActivitiesDTO.Month);
        Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);

    }
    [Fact]
    public async void Should_Not_Save_If_Activities_Days_Dont_Match_Month_Days()
    {
        _monthlyActivitiesDTO.Activities.Add(new ActivityDTO
        {
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = -4,
            MonthlyActivitiesId = 1, 
        });

        var _createdMonthlyActivities = await Helper.Post(_monthlyActivitiesDTO, url, _client, _adminLoginRequestDTO);

        Assert.Null(Helper._APIResponse.Result);
        Assert.NotNull(Helper._APIResponse.ErrorsMessages);
        Assert.Equal("Day cannot exceed the month's max number of days or be less than zero.", Helper._APIResponse.ErrorsMessages.First());
    }
}
