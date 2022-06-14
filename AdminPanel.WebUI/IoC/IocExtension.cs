using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.App.Services;
using AdminPanel.App.Services.Abstract;
using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Concrete;
using AdminPanel.WebUI.Middlewares;
using AdminPanel.WebUI.WebSocketHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.WebUI.IoC
{
    public static class IocExtension
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionDB = configuration.GetConnectionString("DbConnection");
            string connectionCache = configuration.GetConnectionString("CacheConnection");
            services.AddDbContext<EFDbContext>(options => options.UseSqlServer(connectionDB));
            services.AddScoped<IOperatorsRepository, OperatorsRepository>();
            services.AddScoped<IPlayersRepository, PlayersRepository>();
            services.AddScoped<IMessagesRepository, MessagesRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();

            services.AddSingleton<ICacheConnection>(x => RedisConnection.Factory.Connect(connectionCache));

            services.AddScoped<OperatorService>();
            services.AddScoped<PlayerService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<MessageService>();
            services.AddScoped<WebSocketService>();

            services.AddScoped<OperatorBindingDispatcher>();
            services.AddScoped<PlayerBindingDispatcher>();
            return services;
        }

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                        PathString path,
                                                        WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<SocketsRepository>();
            services.AddScoped<WebSocketHandler, OperatorSocketHandler>();
            services.AddScoped<WebSocketHandler, PlayerSocketHandler>();
            return services;
        }
    }
}
