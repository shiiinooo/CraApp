namespace CraApp.Tests.Util;


public static class Helper
{
    public  static APIResponse _APIResponse;


    public async static Task<T> Post<T>(T dto, string url ,HttpClient _client)
    {


        var content = JsonContent.Create(dto);

        var response = await _client.PostAsync(url, content);

        var result = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

         _APIResponse = JsonSerializer.Deserialize<APIResponse>(result, options);

        if (_APIResponse.Result is not null)
        {
            var jsonElement = (JsonElement)_APIResponse.Result;
            var createdActivity = JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), options);
            return createdActivity;
        }
        return default;



    }
    public static async Task<MonthlyActivitiesDTO> PopulateDataBase(HttpClient _client)
    {
        MonthlyActivitiesDTO monthlyActivities = new MonthlyActivitiesDTO { Year = 2024, Month = 06, Activities = [] };
        ActivityDTO _activity = new ActivityDTO
        {
            Project = Project.MyTaraji.ToString(),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = 2,
            //MonthlyActivitiesId = 1

        };
        monthlyActivities.Activities.Add(_activity);
        return await Helper.Post(monthlyActivities, "api/monthlyActivities", _client);

    }
}
