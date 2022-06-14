using AdminPanel.App.Services;
using AdminPanel.Domain.Concrete;
using AdminPanel.Domain.Entities;
using AdminPanel.WebUI.Middlewares;
using AdminPanel.WebUI.Models;
using AdminPanel.WebUI.WebSocketHandlers.Abstract;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.WebSocketHandlers
{
    public class PlayerSocketHandler : WebSocketHandler, ICallBackBinder
    {
        public override SocketClient Client { get; set; }

        private readonly WebSocketService _webSocketService;
        private readonly PlayerBindingDispatcher _bindingDispatcher;

        public PlayerSocketHandler(SocketsRepository socketsRepository,
            WebSocketService webSocketService, PlayerBindingDispatcher dispatcher) : base(socketsRepository)
        {
            _webSocketService = webSocketService;
            _bindingDispatcher = dispatcher;     
        }

        public void Bind(ConcurrentDictionary<string, BindData> binds)
        {
            binds.TryAdd("sendMessage", new BindData()
            {
                callback = OnSendMessageToChat,
                sheme = new { type = "", data = new { message = "" } }
            });
        }

        public override async Task OnConnected(SocketClient client)
        {
            _bindingDispatcher.ApplyBindings(this);
            await base.OnConnected(client);
        }

        public override async Task<SocketClient> HandleRequestConnection(HttpContext context)
        {
            if (!context.Request.QueryString.HasValue)
                return null;

            // Тут дополнительные проверки

            var response = await _webSocketService.CreateSocketClientAsync(context, ClientType.Player);
            return response.Result;
        }

        public override async Task ReceiveAsync(WebSocketReceiveResult result, byte[] buffer)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await _bindingDispatcher.Handle(json);
        }

        public async Task OnSendMessageToChat(dynamic data)
        {
            await _webSocketService.SendMessageAsync(Client, data.message);
        }
    }
}
