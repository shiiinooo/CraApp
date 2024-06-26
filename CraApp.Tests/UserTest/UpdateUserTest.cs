namespace CraApp.Tests.UserTest;

public class UpdateUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private JsonSerializerOptions _options;
    private User _newUser;

    public UpdateUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        // Define JSON serializer options for case-insensitive matching
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _newUser = new User
        {
            UserName = "userToUpdate",
            Name = "name",
            Password = "Admin123@",
            Role = "admin",
        };
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        await ClearDatabaseAsync();
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      
        var createUserContent = JsonContent.Create(_newUser);
        var createResponse = await _client.PostAsync("/users", createUserContent);
        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<APIResponse>(createResponseString, _options);
        var createdUser = JsonSerializer.Deserialize<UserDTO>(createApiResponse.Result.ToString(), _options);

        // Update user data
        var updatedUser = new UserDTO
        {
            Id = createdUser.Id,
            UserName = "updateduser",
            Name = "Updated User",
            Role = "newRole",
        };
        var updateContent = JsonContent.Create(updatedUser);

        // Act
        var updateResponse = await _client.PutAsync($"/users/{createdUser.Id}", updateContent);
        updateResponse.EnsureSuccessStatusCode();
        var updateResponseString = await updateResponse.Content.ReadAsStringAsync();
        var updateApiResponse = JsonSerializer.Deserialize<APIResponse>(updateResponseString, _options);
        var updatedUserResponse = JsonSerializer.Deserialize<UserDTO>(updateApiResponse.Result.ToString(), _options);

        // Assert
        Assert.NotNull(updateApiResponse);
        Assert.True(updateApiResponse.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Empty(updateApiResponse.ErrorsMessages ?? new List<string>());
        Assert.NotNull(updatedUserResponse);
        Assert.Equal(updatedUser.UserName, updatedUserResponse.UserName);
        Assert.Equal(updatedUser.Name, updatedUserResponse.Name);

        await ClearDatabaseAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_NotFound_For_Nonexistent_User()
    {
        // Arrange
        await ClearDatabaseAsync();
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var nonExistentUserId = 999; // Assuming this ID does not exist

        var updateUser = new User
        {
            Id = nonExistentUserId,
            UserName = "updateduser",
            Name = "Updated User",
            Role = "newRole"
        };
        var updateContent = JsonContent.Create(updateUser);

        // Act
        var response = await _client.PutAsync($"/users/{nonExistentUserId}", updateContent);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        await ClearDatabaseAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_BadRequest_For_Invalid_Input()
    {
        // Arrange
        await ClearDatabaseAsync();
        var token = await GetJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createUserContent = JsonContent.Create(_newUser);
        var createResponse = await _client.PostAsync("/users", createUserContent);
        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<APIResponse>(createResponseString, _options);
        var createdUser = JsonSerializer.Deserialize<UserDTO>(createApiResponse.Result.ToString(), _options);

        // Attempt to update with invalid data (empty UserName)
        var invalidUpdateUser = new UserDTO
        {
            Id = createdUser.Id,
            UserName = "", // Invalid input
            Name = "Updated User",
            Role = "admin",
        };
        var invalidUpdateContent = JsonContent.Create(invalidUpdateUser);

        // Act
        var response = await _client.PutAsync($"/users/{createdUser.Id}", invalidUpdateContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await ClearDatabaseAsync();
        _client.Dispose();
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
