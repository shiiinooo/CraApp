using CraApp.Tests.Util;

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


        // First, create a user to ensure there is a user to delete
        var newUser = new User
        {
            UserName = "userToDelete",
            Name = "name",
            Password = "Admin123@",
            Role = Role.admin,
        };
      
        var createdUser = await Helper.Post(newUser, "/users", _client);


        // Act
        var response = await _client.DeleteAsync($"/users/{createdUser.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _client.Dispose();
    }

    [Fact]
    public async Task DeleteUser_Endpoint_Returns_NotFound_For_NonExistent_User()
    {
        // Arrange
        var token = await Helper.GetJwtToken(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //var client = _factory.CreateClient();
        var nonExistentUserId = 9999; // An ID that doesn't exist

        // Act
        var response = await _client.DeleteAsync($"/users/{nonExistentUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.IsSuccess);
        Assert.NotEmpty(apiResponse.ErrorsMessages ?? new List<string>());
    }

   
}
