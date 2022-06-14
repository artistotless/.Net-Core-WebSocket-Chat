using AdminPanel.App.Models.Abstract;
using StackExchange.Redis;

namespace AdminPanel.App.Models
{
    public class RedisConnection : ICacheConnection
    {
        public ConnectionMultiplexer Connection { get; private set; }
        public static ICacheConnection Factory => new RedisConnection();
        public ICacheConnection Connect(string connectionString)
        {
            Connection = ConnectionMultiplexer.Connect(connectionString);
            return this;
        }

    }
}
