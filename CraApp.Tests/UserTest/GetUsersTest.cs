namespace CraApp.Tests.UserTest;

public class GetUsersTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public GetUsersTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    [Fact]
    public async Task GetUsers_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();

        // Define JSON serializer options for case-insensitive matching
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Deserialize APIResponse with options
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, options);
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        // Deserialize the Result to UserDTO
        var jsonElement = (JsonElement)apiResponse.Result;
        var users = JsonSerializer.Deserialize<List<UserDTO>>(jsonElement.GetRawText(), options);

        

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        foreach (var user in users)
        {
            Assert.True(user.Id > 0, "User ID should be greater than 0.");
            Assert.NotNull(user.UserName);
            Assert.NotNull(user.Name);
        }

    }

   

}
