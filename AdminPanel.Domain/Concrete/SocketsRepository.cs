using AdminPanel.Domain.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Concrete
{
    public class SocketsRepository
    {
        private ConcurrentDictionary<string, SocketClient> _sockets = new ConcurrentDictionary<string, SocketClient>();

        public SocketClient GetSocketByUserId(int id, ClientType type)
        {
            return _sockets.FirstOrDefault(p => p.Value.UserId == id && p.Value.Type == type).Value;
        }

        public SocketClient GetSocketBySocketId(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, SocketClient> GetAll()
        {
            return _sockets;
        }

        public string GetId(SocketClient client)
        {
            return client.SocketId;
        }
        public void AddSocket(SocketClient client)
        {
            _sockets.TryAdd(client.SocketId, client);
        }

        public async Task RemoveSocket(string id)
        {
            SocketClient socket;
            _sockets.TryRemove(id, out socket);

            await socket.WebSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocket Service",
                                    cancellationToken: CancellationToken.None);
        }

        public IEnumerable<SocketClient> GetAllByType(ClientType type)
        {
            return _sockets.Values.Where(x => x.Type == type).ToList();
        }
    }
}
