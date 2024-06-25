using System.Text.Json;

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
            UserName = "toto",
            Name = "Test User"
        };
        var content = JsonContent.Create(newUser);

        // Act
        var response = await client.PostAsync("/users", content);

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
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        // Deserialize the Result to UserDTO
        var jsonElement = (JsonElement)apiResponse.Result;
        var createdUser = JsonSerializer.Deserialize<UserDTO>(jsonElement.GetRawText(), options);

        Assert.NotNull(createdUser);
        Assert.Equal(newUser.UserName, createdUser.UserName);
        Assert.Equal(newUser.Name, createdUser.Name);
        Assert.True(createdUser.Id > 0, "User ID should be greater than 0.");
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Invalid_Input()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidUser = new UserDTO
        {
            UserName = "", // Invalid input: empty UserName
            Name = "Test User"
        };
        var content = JsonContent.Create(invalidUser);

        // Act
        var response = await client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_Conflict_For_Duplicate_User()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newUser = new UserDTO
        {
            UserName = "duplicateuser",
            Name = "Duplicate User"
        };
        var content = JsonContent.Create(newUser);

        // Act
        // First attempt to create the user
        await client.PostAsync("/users", content);

        // Second attempt to create the same user
        var response = await client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Missing_Required_Fields()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidUser = new UserDTO
        {
            UserName = "testuser" // Missing Name field
        };
        var content = JsonContent.Create(invalidUser);

        // Act
        var response = await client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
    }

    



}
