
using UnityEngine;

public class Chat : MonoBehaviour
{

    void Start()
    {
        var tokenService = new AuthToken("https://localhost:5001/api/token");
        tokenService.Get((token) =>
        {
            var wsClient = new WebSocketClient("wss://localhost:5001", token);
            wsClient.Start();
        });
    }
}
