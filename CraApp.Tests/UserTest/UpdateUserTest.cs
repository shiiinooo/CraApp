using CraApp.Tests.Util;

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

        
        var createdUser = await Helper.Post(_newUser, "/users", _client);
      

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

        await Helper.CleanUsers(_client, createdUser.Id);
        _client.Dispose();
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_NotFound_For_Nonexistent_User()
    {
        // Arrange

        var token = await Helper.GetJwtToken(_client);
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

        _client.Dispose();
    }

    [Fact]
    public async Task UpdateUser_Endpoint_Returns_BadRequest_For_Invalid_Input()
    {
        // Arrange
        var createdUser = await Helper.Post(_newUser, "/users", _client);
       
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
        await Helper.CleanUsers(_client, createdUser.Id);
        _client.Dispose();
    }

}
