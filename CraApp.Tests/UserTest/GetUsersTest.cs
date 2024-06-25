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

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        if (apiResponse.Result is List<UserDTO> users)
        {
            Assert.NotEmpty(users);
            
            foreach (var user in users)
            {
                Assert.True(user.Id > 0, "User ID should be greater than 0.");
                Assert.NotNull(user.UserName);
                Assert.NotNull(user.Name);
            }
        }



    }
}
