using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Security.Claims;

namespace AdminPanel.Domain.Entities
{
    public enum ClientType { Operator, Player }
    public class SocketClient
    {
        private readonly WebSocket _webSocket;
        private readonly HttpContext _context;

        public SocketClient(WebSocket webSocket, HttpContext context, ClientType type, int id, string name)
        {
            Name = name;
            Type = type;
            _id = id;
            _context = context;
            _webSocket = webSocket;
            SocketId = $"{_id}:{Guid.NewGuid()}";
        }

        public ClientType Type { get; set; }
        public string Name { get; set; }
        public ClaimsPrincipal Principal => _context.User;
        public WebSocket WebSocket => _webSocket;
        public HttpContext Context => _context;
        private int _id;
        public string SocketId { get; private set; }
        public int UserId => _id;


        public override string ToString()
        {
            return SocketId;
        }
    }
}
