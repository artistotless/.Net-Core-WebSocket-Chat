using System;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Web;
using BestHTTP.WebSocket;

public class WebSocketClient
{
    private WebSocket _client;

    public WebSocketClient(string url, string authToken)
    {
        var encodedToken = HttpUtility.UrlEncode(authToken, Encoding.UTF8);
        _client = new WebSocket(new Uri($"{url}/chat?token={authToken}"));

        UIEventer.MessageEnterEvent += SendMessage;
        _client.OnMessage += OnMessageReceived;
        _client.OnOpen += OnServerConnected;
        _client.OnClosed += OnServerDisconnected;
    }

    public void Start()
    {
        _client.Open();
    }

    private void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }
        var requestScheme = new SendMessageScheme()
        {
            type = "sendMessage",
            data = new SendMessageScheme.Data() { message = message }
        };

        string json = JsonConvert.SerializeObject(requestScheme);
        _client.Send(json);
    }

    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        var response = JsonConvert.DeserializeObject<ChatMessageScheme>(message);
        Debug.Log(
            $"Type: {response.type} \n" +
            $"MessageId: {response.data.messageId} \n" +
            $"Operator: {response.data.@operator.name} \n" +
            $"Text: {response.data.text} \n" +
            $"Time: {response.data.timeStamp}");
    }

    private void OnServerConnected(WebSocket webSocket)
    {
        Debug.Log("Server connected");
    }

    private void OnServerDisconnected(WebSocket webSocket, UInt16 code, string message)
    {
        Debug.Log("Server disconnected");
    }
}
