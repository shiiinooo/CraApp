using CraApp.Tests.Util;

namespace CraApp.Tests.UserTest;

public class CreateUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private UserDTO _newUser;
    private JsonSerializerOptions _options;
    private readonly string url = "/users";
    private LoginRequestDTO _adminLoginRequestDTO;
    public CreateUserTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _newUser = new UserDTO
        {
            UserName = "name",
            Name = "name",
            Password = "Admin123@",
            Role = Role.admin.ToString(),
        };

        _adminLoginRequestDTO = new LoginRequestDTO
        {
            UserName = "shiinoo",
            Password = "Password123#"
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
    
        var createdUser = await Helper.Post(_newUser, url, _client, _adminLoginRequestDTO);

        Assert.NotNull(Helper._APIResponse);
        Assert.Equal(HttpStatusCode.Created, Helper._APIStatusCode);
        Assert.Empty(Helper._APIResponse.ErrorsMessages ?? new List<string>());

        // Deserialize the Result to UserDTO


        Assert.NotNull(createdUser);
        Assert.Equal(_newUser.UserName, createdUser.UserName);
        Assert.Equal(_newUser.Name, createdUser.Name);
        Assert.Equal(_newUser.Role, createdUser.Role);
        //Assert.True(createdUser.Id > 0, "User ID should be greater than 0.");


        await Helper.CleanUsers(_client, createdUser.Id, _adminLoginRequestDTO);
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Invalid_Input()
    {
        // Arrange

        var invalidUser = new UserDTO
        {
            UserName = "", // Invalid input: empty UserName
            Name = "Test User",
            Password = "Admin123",
            Role = Role.admin.ToString()
        };

        var userDTO = await Helper.Post(invalidUser, url, _client, _adminLoginRequestDTO);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, Helper._APIStatusCode);

        Assert.NotNull(Helper._APIResponse);
        Assert.NotEmpty(Helper._APIResponse.ErrorsMessages ?? new List<string>());
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_Conflict_For_Duplicate_User()
    {
        // Arrange
        
        var newUser = new UserDTO
        {
            UserName = "duplicateuser",
            Name = "Duplicate User",
            Password = "Admin123",
            Role = Role.admin.ToString()
        };
       

        // Act
        // First attempt to create the user
        var createduser = await Helper.Post(newUser, "/users", _client, _adminLoginRequestDTO);

        // Second attempt to create the same user
        var result = await Helper.Post(newUser, "/users", _client, _adminLoginRequestDTO);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, Helper._APIStatusCode);

        

        Assert.NotNull(Helper._APIResponse);
        Assert.NotEmpty(Helper._APIResponse.ErrorsMessages ?? new List<string>());

        await Helper.CleanUsers(_client, createduser.Id, _adminLoginRequestDTO);
        _client.Dispose();
    }

    [Fact]
    public async Task CreateUser_Endpoint_Returns_BadRequest_For_Missing_Required_Fields()
    {
        
        var invalidUser = new UserDTO
        {
            UserName = "testuser", // Missing Name field
            Password = "Admin123",
            Role = Role.admin.ToString()
        };  
      

        // Act
        var response = await Helper.Post(invalidUser, "/users",_client, _adminLoginRequestDTO);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, Helper._APIStatusCode);

        Assert.NotNull(Helper._APIResponse);
        Assert.NotEmpty(Helper._APIResponse.ErrorsMessages ?? new List<string>());
    }

   
   

   

}
