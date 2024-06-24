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
        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
    }
}
