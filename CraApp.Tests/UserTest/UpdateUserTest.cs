using CraApp.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CraApp.Tests.UserTest;

public class UpdateUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UpdateUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_Correct_Response()
    {
        // Arrange
        await ClearDatabaseAsync();
        var client = _factory.CreateClient();

        // Create a new user first
        var newUser = new UserDTO
        {
            UserName = "testuser",
            Name = "Test User"
        };
        var createUserContent = JsonContent.Create(newUser);
        var createResponse = await client.PostAsync("/users", createUserContent);
        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        // Define JSON serializer options for case-insensitive matching
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var createApiResponse = JsonSerializer.Deserialize<APIResponse>(createResponseString, options);
        var createdUser = JsonSerializer.Deserialize<UserDTO>(createApiResponse.Result.ToString(), options);

        // Update user data
        var updatedUser = new UserDTO
        {
            Id = createdUser.Id,
            UserName = "updateduser",
            Name = "Updated User"
        };
        var updateContent = JsonContent.Create(updatedUser);

        // Act
        var updateResponse = await client.PutAsync($"/users/{createdUser.Id}", updateContent);
        updateResponse.EnsureSuccessStatusCode();
        var updateResponseString = await updateResponse.Content.ReadAsStringAsync();
        var updateApiResponse = JsonSerializer.Deserialize<APIResponse>(updateResponseString, options);
        var updatedUserResponse = JsonSerializer.Deserialize<UserDTO>(updateApiResponse.Result.ToString(), options);

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
        var client = _factory.CreateClient();
        var nonExistentUserId = 999; // Assuming this ID does not exist

        var updateUser = new UserDTO
        {
            Id = nonExistentUserId,
            UserName = "updateduser",
            Name = "Updated User"
        };
        var updateContent = JsonContent.Create(updateUser);

        // Act
        var response = await client.PutAsync($"/users/{nonExistentUserId}", updateContent);

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
        var client = _factory.CreateClient();

        // Create a new user first
        var newUser = new UserDTO
        {
            UserName = "testuser",
            Name = "Test User"
        };
        var createUserContent = JsonContent.Create(newUser);
        var createResponse = await client.PostAsync("/users", createUserContent);
        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        // Define JSON serializer options for case-insensitive matching
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var createApiResponse = JsonSerializer.Deserialize<APIResponse>(createResponseString, options);
        var createdUser = JsonSerializer.Deserialize<UserDTO>(createApiResponse.Result.ToString(), options);

        // Attempt to update with invalid data (empty UserName)
        var invalidUpdateUser = new UserDTO
        {
            Id = createdUser.Id,
            UserName = "", // Invalid input
            Name = "Updated User"
        };
        var invalidUpdateContent = JsonContent.Create(invalidUpdateUser);

        // Act
        var response = await client.PutAsync($"/users/{createdUser.Id}", invalidUpdateContent);

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


}
