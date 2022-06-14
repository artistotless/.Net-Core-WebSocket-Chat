using AdminPanel.App.Helpers;
using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.Domain.Concrete;
using AdminPanel.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AdminPanel.App.Services
{
    public class WebSocketService
    {
        private readonly PlayerService _player;
        private readonly OperatorService _operator;
        private readonly MessageService _messages;
        private readonly SocketsRepository _sockets;

        public WebSocketService(PlayerService player, OperatorService @operator,
            MessageService messages, SocketsRepository sockets)
        {
            _player = player;
            _operator = @operator;
            _messages = messages;
            _sockets = sockets;
        }

        public async Task<Response<bool>> SendMessageAsync(SocketClient client, string message)
        {
            var response = new Response<bool>();
            try
            {
                var addMessageResponse = await _messages.AddAsync(message, client.UserId);
                int messageId = -1;

                if (addMessageResponse.HasError)
                {
                    return response;
                }

                messageId = addMessageResponse.Result.MessageID;

                // TODO: избавиться от ответственности за определение схемы ответа
                var scheme = new
                {
                    type = "chatMessage",
                    data = new
                    {
                        messageId = messageId,
                        text = message,
                        timeStamp = addMessageResponse.Result.TimeStamp.ToString(),
                        player = new { id = client.UserId, name = client.Name },
                    }
                };

                var json = JsonConvert.SerializeObject(scheme);
                var operatorsSockets = _sockets.GetAllByType(ClientType.Operator);
                var sendingReponse = await SendToSocketAsync(operatorsSockets, json);

                response = new Response<bool>()
                {
                    HasError = sendingReponse.HasError,
                    Message = sendingReponse.HasError ? Error.WEBSOCKET : string.Empty
                };
            }
            catch
            {
                Error.SetError(response, Error.WEBSOCKET);
            }
            return response;
        }

        public async Task<Response<bool>> SendMessageToPlayerAsync(SocketClient client, int playerId, string message)
        {
            var response = new Response<bool>();
            try
            {
                var addMessageResponse = await _messages.AddAsync(message, playerId, client.UserId, true);
                int messageId = -1;

                if (addMessageResponse.HasError)
                {
                    return response;
                }

                messageId = addMessageResponse.Result.MessageID;
                // TODO: избавиться от ответственности за определение схемы ответа
                var scheme = new
                {
                    type = "chatMessage",
                    data = new
                    {
                        messageId = messageId,
                        text = message,
                        timeStamp = addMessageResponse.Result.TimeStamp.ToString(),
                        @operator = new { name = client.Name }
                    }
                };

                var json = JsonConvert.SerializeObject(scheme);
                var playerSocket = _sockets.GetSocketByUserId(playerId, ClientType.Player);
                var sendingReponse = await SendToSocketAsync(new[] { client, playerSocket }, json);

                response = new Response<bool>()
                {
                    HasError = sendingReponse.HasError,
                    Message = sendingReponse.HasError ? Error.WEBSOCKET : string.Empty
                };
            }
            catch
            {
                Error.SetError(response, Error.WEBSOCKET);
            }
            return response;
        }

        public async Task<Response<bool>> SetMessageReadAsync(SocketClient client, int messageId)
        {
            var response = await _messages.SetMessageRead(messageId);
            return new Response<bool>()
            {
                HasError = response.HasError,
                Message = response.HasError ? Error.WEBSOCKET : string.Empty
            };
        }

        public async Task<Response<bool>> GetMessagesAsync(SocketClient client, int page = 0, int playerId = 0)
        {
            var res = await _messages.GetDialogsAsync();
            var res2 = res.Result;
            var response = new Response<bool>();
            try
            {
                var messagesResponse = await _messages.GetAllByPlayerAsync(playerId, page);
                var messagesCountResponse = await _messages.CountByPlayerAsync(playerId);

                if (messagesResponse.HasError || messagesCountResponse.HasError)
                {
                    return response;
                }

                var messagesCount = messagesCountResponse.Result;
                var messages = messagesResponse.Result.ToList();

                // TODO: избавиться от ответственности за определение схемы ответа
                var scheme = new
                {
                    type = "getMessages",
                    data = new
                    {
                        count = messagesCount,
                        messages = messages.Select(x => new
                        {
                            messageId = x.MessageID,
                            read = x.Read,
                            text = x.Text,
                            timeStamp = x.TimeStamp.ToString(),
                            @operator = new { name = x.Operator != null ? x.Operator.Name : string.Empty },
                            player = new { id = x.PlayerID, name = x.Player.Name }
                        })
                    }
                };

                var json = JsonConvert.SerializeObject(scheme);
                var sendingReponse = await SendToSocketAsync(client, json);

                response = new Response<bool>()
                {
                    HasError = sendingReponse.HasError,
                    Message = sendingReponse.HasError ? Error.WEBSOCKET : string.Empty
                };
            }
            catch
            {
                Error.SetError(response, Error.WEBSOCKET);
            }
            return response;
        }

        public async Task<Response<SocketClient>> CreateSocketClientAsync(HttpContext context, ClientType type)
        {
            var response = new Response<SocketClient>();
            try
            {
                if (type == ClientType.Player)
                {
                    response.Result = await CreatePlayerSocketClient(context);
                }

                else if (type == ClientType.Operator)
                {
                    response.Result = await CreateOperatorSocketClient(context);
                }
            }
            catch
            {
                Error.SetError(response, Error.UNKNOWN);
            }
            return response;
        }


        private async Task<SocketClient> CreatePlayerSocketClient(HttpContext context)
        {
            var gettingPlayerResponse = await _player.GetByContextAsync(context);
            SocketClient client = null;
            if (!gettingPlayerResponse.HasError)
            {
                var player = gettingPlayerResponse.Result;
                client = await CreateSocketClientAsync(context, ClientType.Player, player.PlayerID, player.Name);
            }

            if (client == null)
                throw new WebSocketException(Error.WEBSOCKET);

            return client;
        }
        private async Task<SocketClient> CreateOperatorSocketClient(HttpContext context)
        {
            var gettingOperatorResponse = await _operator.GetByContextAsync(context);
            SocketClient client = null;
            if (!gettingOperatorResponse.HasError)
            {
                var @operator = gettingOperatorResponse.Result;
                client = await CreateSocketClientAsync(context, ClientType.Operator, @operator.OperatorID, @operator.Name);
            }

            if (client == null)
                throw new WebSocketException(Error.WEBSOCKET);

            return client;
        }
        private async Task<SocketClient> CreateSocketClientAsync(HttpContext context, ClientType type, int id, string name)
        {
            return new SocketClient(webSocket: await context.WebSockets.AcceptWebSocketAsync(),
                                  context: context,
                                  type: type,
                                  id: id,
                                  name: name);
        }
        private async Task<Response<bool>> SendToSocketAsync(SocketClient reciever, string json)
        {
            var response = new Response<bool>();
            if (reciever == null)
                return response;
            try
            {
                var segment = json.ToArraySegment();
                await reciever.WebSocket.SendAsync(segment, WebSocketMessageType.Text, true,
                    CancellationToken.None);
                response.Result = true;
            }

            catch
            {
                Error.SetError(response, Error.WEBSOCKET);
            }
            return response;
        }
        private async Task<Response<bool>> SendToSocketAsync(IEnumerable<SocketClient> recievers, string json)
        {
            foreach (var reciever in recievers)
            {
                var result = await SendToSocketAsync(reciever, json);
            }
            return new Response<bool>();
        }
    }
}
