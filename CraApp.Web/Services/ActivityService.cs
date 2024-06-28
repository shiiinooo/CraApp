namespace CraApp.Web.Services;

public class ActivityService : BaseService, IActivityService
{
    private readonly IHttpClientFactory _clientFactory;
    private string apiBaseUrl;

    public ActivityService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
    {
        _clientFactory = clientFactory;
        apiBaseUrl = configuration.GetValue<string>("ServiceUrls:CraAPI");
    }

    public Task<T> CreateAsync<T>(ActivityCreateDTO dto, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = apiBaseUrl + "/api/activity",
            Token = token
        });
    }

    public Task<T> UpdateAsync<T>(ActivityUpdateDTO dto, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = apiBaseUrl + "/api/activity/" + dto.Id,
            Token = token
        });
    }

    public Task<T> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = apiBaseUrl + "/api/activity/" + id,
            Token = token
        });
    }

    public Task<T> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = apiBaseUrl + "/api/activity",
            Token = token
        });
    }

    public Task<T> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = apiBaseUrl + "/api/activity/" + id,
            Token = token
        });
    }
}