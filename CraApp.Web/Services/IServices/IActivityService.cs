namespace CraApp.Web.Services.IServices;

public interface IActivityService
{
    Task<T> CreateAsync<T>(ActivityCreateDTO dto, string token);
    Task<T> UpdateAsync<T>(ActivityUpdateDTO dto, string token);
    Task<T> DeleteAsync<T>(int id, string token);
    Task<T> GetAllAsync<T>(string token);
    Task<T> GetAsync<T>(int id, string token);
}