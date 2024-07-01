using CraApp.Features.UserFeature;
using CraApp.Repository.IRepository;
using CraApp.Tests.Util;
using Moq;

namespace CraApp.Tests.UserTest;

public class GetUsersTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private JsonSerializerOptions _options;
    private LoginRequestDTO _adminLoginRequestDTO;

    public GetUsersTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var userRepositoryMock = new Mock<IUserRepository>();
                userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(new List<User>());

                services.AddSingleton(userRepositoryMock.Object);
            });
        });
        _client = _factory.CreateClient();
        // Define JSON serializer options for case-insensitive matching
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _adminLoginRequestDTO = new LoginRequestDTO
        {
            UserName = "shiinoo",
            Password = "Password123#"
        };

    }
    [Fact]
    public async Task GetUsers_Endpoint_Returns_Correct_Response_for_authorised_existing_user()
    {
        // Arrange
        var token = await Helper.GetJwtToken(_client, _adminLoginRequestDTO);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();

        // Deserialize APIResponse with options
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, _options);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

    }

    [Fact]
    public async Task GetUsers_Endpoint_Returns_Unauthorized_When_No_Token()
    {
        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_Endpoint_Returns_Forbidden_For_Insufficient_Permissions()
    {
        // Arrange
        var loginRequest = new LoginRequestDTO
        {
            UserName = "user",
            Password = "user"
        };
        var token = await Helper.GetJwtToken(_client, loginRequest);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_Endpoint_Returns_Empty_List_When_No_Users()
    {
        // Arrange
        var token = await Helper.GetJwtToken(_client, _adminLoginRequestDTO);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseString = await response.Content.ReadAsStringAsync();

        // Deserialize APIResponse with options
        var apiResponse = JsonSerializer.Deserialize<APIResponse>(responseString, _options);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(apiResponse.ErrorsMessages ?? new List<string>());

        // Explicitly handle JsonElement deserialization
        var users = ((JsonElement)apiResponse.Result).Deserialize<IEnumerable<GetUsersResult>>(_options);
        Assert.Empty(users);
    }

}
