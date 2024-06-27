
namespace CraApp.Web.Services
{
    public interface IMonthlyActivitiesService
    {
        Task<T> CreateAsync<T>(MonthlyActivityCreateDTO dto, string token);
        Task<T> UpdateAsync<T>(MonthlyActivityUpdateDTO dto, string token);
        Task<T> DeleteAsync<T>(int id, string token);
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> GetActivitiesByMonthAndUser<T>(int UserId, int Month, string token);
    }
}
