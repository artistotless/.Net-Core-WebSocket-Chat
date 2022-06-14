using AdminPanel.WebUI.Models;
using AdminPanel.WebUI.WebSocketHandlers.Abstract;
using System.Collections.Concurrent;

namespace AdminPanel.WebUI.WebSocketHandlers
{
    public class PlayerBindingDispatcher : BindingDispatcher
    {
        public override ConcurrentDictionary<string, BindData> BindCallbacks { get; set; }

        public PlayerBindingDispatcher()
        {
            BindCallbacks = new ConcurrentDictionary<string, BindData>();
        }
    }
}