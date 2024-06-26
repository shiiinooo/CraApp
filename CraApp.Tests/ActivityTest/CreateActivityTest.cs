﻿using CraApp.Tests.Util;

namespace CraApp.Tests.ActivityTest
{
    public class CreateActivityTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string url = "/api/activity";
        private LoginRequestDTO _adminLoginRequestDTO;

        public CreateActivityTest()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            _adminLoginRequestDTO = new LoginRequestDTO
            {
                UserName = "shiinoo",
                Password = "Password123#"
            };
        }

        [Fact]
        public async void Should_Save_Activity()
        {
            MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

            ActivityDTO _activity = new ActivityDTO
            {
                Project = Project.MyTaraji.ToString(),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                Day = 2,
                MonthlyActivitiesId = monthlyActivitiesDTO.Id
            };

            var createdActivity = await Helper.Post(_activity, url, _client, _adminLoginRequestDTO);

            // Assert the values
            Assert.Equal(HttpStatusCode.Created, Helper._APIStatusCode);
            Assert.NotNull(Helper._APIResponse);
            Assert.NotNull(Helper._APIResponse.Result);
            Assert.Empty(Helper._APIResponse.ErrorsMessages);

            Assert.Equal(_activity.StartTime, createdActivity.StartTime);
            Assert.Equal(_activity.EndTime, createdActivity.EndTime);
            Assert.Equal(_activity.Project, createdActivity.Project);

            await Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);
        }

        [Fact]
        public async void Should_Throw_Exception_For_StartTime_Greater_Than_EndTime()
        {
            MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

            ActivityDTO _activity = new ActivityDTO
            {
                Project = Project.MyTaraji.ToString(),
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Day = 2,
                MonthlyActivitiesId = monthlyActivitiesDTO.Id
            };

            var result = await Helper.Post(_activity, url, _client, _adminLoginRequestDTO);

            Assert.Equal(HttpStatusCode.BadRequest, Helper._APIStatusCode);
            Assert.NotNull(Helper._APIResponse);
            Assert.Null(Helper._APIResponse.Result);
            Assert.NotEmpty(Helper._APIResponse.ErrorsMessages);

            await Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);
        }

        [Fact]
        public async void Should_Not_Save_Activity_if_MaxHours_exceeded_Limits()
        {
            MonthlyActivitiesDTO monthlyActivitiesDTO = await Helper.PopulateDataBase(_client);

            ActivityDTO _activity = new ActivityDTO
            {
                Project = Project.MyTaraji.ToString(),
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                Day = 2,
                MonthlyActivitiesId = monthlyActivitiesDTO.Id
            };

            var result = await Helper.Post(_activity, url, _client, _adminLoginRequestDTO);

            Assert.Equal(HttpStatusCode.BadRequest, Helper._APIStatusCode);
            Assert.NotNull(Helper._APIResponse);
            Assert.Null(Helper._APIResponse.Result);
            Assert.NotEmpty(Helper._APIResponse.ErrorsMessages);

            await Helper.CleanMonthlyActivities(_client, monthlyActivitiesDTO.Id, _adminLoginRequestDTO);
        }
    }
}
