using CraApp.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CraApp.Tests.UserTest;

public class DeleteUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DeleteUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task DeleteUser_Endpoint_Returns_NoContent_For_Successful_Deletion()
    {
        // Arrange
        await ClearDatabaseAsync();
        var client = _factory.CreateClient();

        // First, create a user to ensure there is a user to delete
        var newUser = new UserDTO
        {
            UserName = "userToDelete1",
            Name = "User To Delete"
        };
        var content = JsonContent.Create(newUser);
        var createResponse = await client.PostAsync("/users", content);
        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(createResponseString, options);
        var jsonElement = (JsonElement)apiResponse.Result;
        var createdUser = JsonSerializer.Deserialize<UserDTO>(jsonElement.GetRawText(), options);

        // Act
        var response = await client.DeleteAsync($"/users/{createdUser.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await ClearDatabaseAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task DeleteUser_Endpoint_Returns_NotFound_For_NonExistent_User()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentUserId = 9999; // An ID that doesn't exist

        // Act
        var response = await client.DeleteAsync($"/users/{nonExistentUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

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
}
