using AdminPanel.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Middlewares
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketManagerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IEnumerable<WebSocketHandler> webSocketHandlers)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            WebSocketHandler handler = null;
            SocketClient client = null;

            foreach (var item in webSocketHandlers)
            {
                client = await item.HandleRequestConnection(context);
                if (client != null)
                {
                    handler = item;
                    break;
                }
            }

            if (handler == null)
            {
                return;
            }

            await handler.OnConnected(client);

            await Receive(client, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await handler.ReceiveAsync(result,buffer);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await handler.OnDisconnected(client);
                    return;
                }

            });
        }

        private async Task Receive(SocketClient client, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (client.WebSocket.State == WebSocketState.Open)
            {
                var result = await client.WebSocket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                        cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
