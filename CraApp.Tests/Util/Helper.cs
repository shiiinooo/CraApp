namespace CraApp.Tests.Util;


public static class Helper
{
    public  static APIResponse _APIResponse;
    public static HttpStatusCode _APIStatusCode;

    public async static Task<T> Post<T>(T dto, string url ,HttpClient _client)
    {
        var token = await Helper.GetJwtToken(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = JsonContent.Create(dto);

        var response = await _client.PostAsync(url, content);

        _APIStatusCode = response.StatusCode;
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

    public static async Task CleanMonthlyActivities(HttpClient _client, int Id)
    {
        var token = await GetJwtToken(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"/api/monthlyActivities/{Id}");
    }

    public static async Task CleanUsers(HttpClient _client, int Id)
    {
        var token = await GetJwtToken(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _client.DeleteAsync($"/users/{Id}");

    }

    public static async Task<MonthlyActivitiesDTO> PopulateDataBase(HttpClient _client)
    {
        ActivityDTO _activity = new ActivityDTO
        {
            Project = Project.MyTaraji.ToString(),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            Day = 2,

        };

        MonthlyActivitiesDTO monthlyActivities = new MonthlyActivitiesDTO { Year = 2023, Month = 12,
            Activities = new List<ActivityDTO> { _activity } ,
            UserId =1};
        
      

        return await Helper.Post(monthlyActivities, "api/monthlyActivities", _client);

    }
    public static  async Task<string> GetJwtToken(HttpClient _client)
    {
        var loginRequest = new
        {
            UserName = "shiinoo",
            Password = "Password123#"
        };

        var response = await _client.PostAsJsonAsync("/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, options);

        if (apiResponse?.Result != null)
        {
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(apiResponse.Result.ToString(), options);
            return loginResponse?.Token;
        }

        throw new InvalidOperationException("Failed to retrieve JWT token");
    }
}
