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
    public class OperatorSocketHandler : WebSocketHandler, ICallBackBinder
    {
        public override SocketClient Client { get; set; }

        private readonly WebSocketService _webSocketService;
        private readonly OperatorBindingDispatcher _bindingDispatcher;

        public OperatorSocketHandler(SocketsRepository socketsRepository,
            WebSocketService webSocketService, OperatorBindingDispatcher dispatcher) : base(socketsRepository)
        {
            _webSocketService = webSocketService;
            _bindingDispatcher = dispatcher;
        }

        public void Bind(ConcurrentDictionary<string, BindData> binds)
        {
            binds.TryAdd("sendMessage", new BindData()
            {
                callback = OnSendMessage,
                sheme = new { type = "", data = new { playerId = 0, message = "" } }
            });

            binds.TryAdd("getMessages", new BindData()
            {
                callback = OnGetMessages,
                sheme = new { type = "", data = new { page = 0, playerId = 0 } }
            });

            binds.TryAdd("setMessageRead", new BindData()
            {
                callback = OnSetMessageRead,
                sheme = new { type = "", data = new { messageId = 0 } }
            });
        }

        public override async Task OnConnected(SocketClient client)
        {
            _bindingDispatcher.ApplyBindings(this);
            await base.OnConnected(client);
        }

        public override async Task<SocketClient> HandleRequestConnection(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated || context.User.IsInRole("Гость"))
                return null;

            // Тут дополнительные проверки

            var response = await _webSocketService.CreateSocketClientAsync(context, ClientType.Operator);
            return response.Result;
        }

        public override async Task ReceiveAsync(WebSocketReceiveResult result, byte[] buffer)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await _bindingDispatcher.Handle(json);
        }

        private async Task OnSendMessage(dynamic data)
        {
            // Только пользователь админки с правом "CanCreate" может отправить сообщение
            if (Client.Context.User.HasClaim("CanCreate", "True"))
                await _webSocketService.SendMessageToPlayerAsync(Client, data.playerId, data.message);
        }

        private async Task OnGetMessages(dynamic data)
        {
            await _webSocketService.GetMessagesAsync(Client, data.page, data.playerId);
        }

        private async Task OnSetMessageRead(dynamic data)
        {
            // Только пользователь админки с правом "CanCreate"
            // может отметить сообщение прочитанных
            if (Client.Context.User.HasClaim("CanCreate", "True"))
                await _webSocketService.SetMessageReadAsync(Client, data.messageId);
        }
    }
}
