
using AdminPanel.App.Models.Abstract;
using System.Threading.Tasks;

namespace AdminPanel.App.Services.Abstract
{
    public interface ICacheService
    {
        Task<Response<string>> GetValueAsync(string key);
        Task<Response<bool>> SetValueAsync(string key, string value);
        Task<Response<bool>> ExistAsync(string key);
        Task<Response<long>> DecrementValueAsync(string key);
    }
}
