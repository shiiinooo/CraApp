namespace CraApp.Web.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private string craUrl;

    public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        craUrl = configuration.GetValue<string>("ServiceUrls:CraAPI");

    }

    public Task<T> LoginAsync<T>(LoginRequestDTO obj)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = craUrl + "/login"
        });
    }

    public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = craUrl + "/register"
        });
    }
}
