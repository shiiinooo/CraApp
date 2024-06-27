using CraApp.Web.Models.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace CraApp.Web.Services
{
    public class MonthlyActivitiesService : BaseService, IMonthlyActivitiesService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string apiBaseUrl;

        public MonthlyActivitiesService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            apiBaseUrl = configuration.GetValue<string>("ServiceUrls:CraAPI");
        }

        public Task<T> CreateAsync<T>(MonthlyActivityCreateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = apiBaseUrl + "/api/monthlyActivities",
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(MonthlyActivityUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = apiBaseUrl + "/api/monthlyActivities/" + dto.Id,
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = apiBaseUrl + "/api/monthlyActivities/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = apiBaseUrl + "/api/monthlyActivities",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = apiBaseUrl + "/api/monthlyActivities/" + id,
                Token = token
            });
        }

       

        public Task<T> GetActivitiesByMonthAndUser<T>(int UserId, int Month, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = apiBaseUrl + $"/api/activity/{UserId}/{Month}",
                Token = token
            });
        }

    }
}
