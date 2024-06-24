
namespace CraApp.Tests.CraService;

public class CreateActivityTest
{
    //private readonly Mock<CreateActivity> _createActivityMock;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private ActivityDTO _activity;
    private APIResponse _APIResponse;

    public CreateActivityTest()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    [Fact]
    public async void Should_Save_Activity()
    {

        //var response = _client.PostAsync("/api/activity");

    }

    [Fact]
    public void Should_Throw_Exception_For_StartTime_Greater_Than_EndTime()
    {
        
    }

    [Fact]
    public void Should_Not_Save_Activity_if_MaxHours_exceded_Limits()
    {

    }

    [Fact]
    public void Should_Not_Save_Activity_If_It_Already_Exists()
    {

    }
}
