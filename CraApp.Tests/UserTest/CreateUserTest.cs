namespace CraApp.Tests.UserTest;

public class CreateUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private User _newUser;
    private JsonSerializerOptions _options;

    public CreateUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _newUser = new User
        {
            UserName = "name",
            Name = "name",
            Password = "Admin123@",
            Role = "admin",
        };

        // Define JSON serializer options for case-insensitive matching
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        await ClearDatabaseAsync();
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var content = JsonContent.Create(_newUser);

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();
       
        // Deserialize APIResponse with options
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, _options);
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        // Deserialize the Result to UserDTO
        var jsonElement = (JsonElement)apiResponse.Result;
        var createdUser = JsonSerializer.Deserialize<UserDTO>(jsonElement.GetRawText(), _options);

        Assert.NotNull(createdUser);
        Assert.Equal(_newUser.UserName, createdUser.UserName);
        Assert.Equal(_newUser.Name, createdUser.Name);
        Assert.Equal(_newUser.Role, createdUser.Role);
        Assert.True(createdUser.Id > 0, "User ID should be greater than 0.");

        await ClearDatabaseAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Invalid_Input()
    {
        // Arrange
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var invalidUser = new User
        {
            UserName = "", // Invalid input: empty UserName
            Name = "Test User",
            Password = "Admin123",
            Role = "admin"
        };
        var content = JsonContent.Create(invalidUser);

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_Conflict_For_Duplicate_User()
    {
        // Arrange
        await ClearDatabaseAsync();
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //var client = _factory.CreateClient();
        var newUser = new User
        {
            UserName = "duplicateuser",
            Name = "Duplicate User",
            Password = "Admin123",
            Role = "admin"
        };
        var content = JsonContent.Create(newUser);

        // Act
        // First attempt to create the user
        await _client.PostAsync("/users", content);

        // Second attempt to create the same user
        var response = await _client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());

        await ClearDatabaseAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Missing_Required_Fields()
    {
        // Arrange
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //var client = _factory.CreateClient();
        var invalidUser = new User
        {
            UserName = "testuser", // Missing Name field
            Password = "Admin123",
            Role = "admin"
        };
        var content = JsonContent.Create(invalidUser);

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task ClearDatabaseAsync()
    {
        // Implement logic to clear or reset database state
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
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
