namespace CraApp.Tests.UserTest;

public class GetUsersTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private JsonSerializerOptions _options;

    public GetUsersTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        // Define JSON serializer options for case-insensitive matching
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
    [Fact]
    public async Task GetUsers_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();

        // Deserialize APIResponse with options
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, _options);
        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        // Deserialize the Result to UserDTO
        var jsonElement = (JsonElement)apiResponse.Result;
        var users = JsonSerializer.Deserialize<List<UserDTO>>(jsonElement.GetRawText(), _options);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        foreach (var user in users)
        {
            Assert.True(user.Id > 0, "User ID should be greater than 0.");
            Assert.NotNull(user.UserName);
            Assert.NotNull(user.Name);
        }
    }

    private async Task<string> GetJwtToken()
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
