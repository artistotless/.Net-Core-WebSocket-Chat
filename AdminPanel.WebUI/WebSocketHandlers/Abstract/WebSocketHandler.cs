using AdminPanel.Domain.Concrete;
using AdminPanel.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Middlewares
{
    public abstract class WebSocketHandler
    {
        protected SocketsRepository SocketsRepository { get; set; }
        public abstract SocketClient Client { get; set; }

        public WebSocketHandler(SocketsRepository repository)
        {
            SocketsRepository = repository;
        }

        public virtual async Task OnConnected(SocketClient client)
        {
            Client = client;
            SocketsRepository.AddSocket(Client);
        }

        public virtual async Task OnDisconnected(SocketClient client)
        {
            await SocketsRepository.RemoveSocket(SocketsRepository.GetId(client));
        }

        public abstract Task ReceiveAsync(WebSocketReceiveResult result, byte[] buffer);
        public abstract Task<SocketClient> HandleRequestConnection(HttpContext context);

    }
}
