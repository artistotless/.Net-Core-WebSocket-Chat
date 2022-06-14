using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.App.Services.Abstract;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace AdminPanel.App.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly ConnectionMultiplexer _connection;

        public RedisCacheService(ICacheConnection connection)
        {
            _connection = (connection as RedisConnection).Connection;
        }

        public async Task<Response<bool>> ExistAsync(string key)
        {
            var db = _connection.GetDatabase();
            return new Response<bool>()
            {
                Result = await db.KeyExistsAsync(key)
            };
        }

        public async Task<Response<string>> GetValueAsync(string key)
        {
            var db = _connection.GetDatabase();
            return new Response<string>()
            {
                Result = await db.StringGetAsync(key)          
            };
        }

        public async Task<Response<bool>> SetValueAsync(string key, string value)
        {
            var db = _connection.GetDatabase();
            return new Response<bool>()
            {
                Result = await db.StringSetAsync(key, value)
            };
        }

        public async Task<Response<long>> DecrementValueAsync(string key)
        {
            var db = _connection.GetDatabase();
            return new Response<long>()
            {
                Result = await db.StringDecrementAsync(key)
            };
        }
    }
}
