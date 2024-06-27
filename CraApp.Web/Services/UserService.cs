namespace CraApp.Web.Services;

public class UserService : BaseService, IUserService
{
    private readonly IHttpClientFactory _clientFactory;
    private string villaUrl;

    public UserService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrls:CraAPI");

    }

    public Task<T> CreateAsync<T>(UserCreateDTO dto, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = villaUrl + "/users",
            Token = token
        });
    }

    public Task<T> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = villaUrl + "/users/" + id,
            Token = token
        });
    }

    public Task<T> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = villaUrl + "/users",
            Token = token
        });
    }

    public Task<T> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = villaUrl + "/user/" + id,
            Token = token
        });
    }

    public Task<T> UpdateAsync<T>(UserUpdateDTO dto, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = villaUrl + "/users/" + dto.Id,
            Token = token
        });
    }

    public Task<T> GetUserWithActivitiesAsync<T>(int userId, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = villaUrl + "/users/" + userId + "/monthly-activities",
            Token = token
        });
    }

}