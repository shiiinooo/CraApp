namespace CraApp.Tests.UserTest;

public class CreateUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CreateUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newUser = new UserDTO
        {
            UserName = "testuser",
            Name = "Test User"
        };
        var content = JsonContent.Create(newUser);

        // Act
        var response = await client.PostAsync("/users", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        if (apiResponse.Result is UserDTO createdUser)
        {
            Assert.NotNull(createdUser);
            Assert.Equal(newUser.UserName, createdUser.UserName);
            Assert.Equal(newUser.Name, createdUser.Name);
            Assert.True(createdUser.Id > 0, "User ID should be greater than 0.");
        }
    }

}
