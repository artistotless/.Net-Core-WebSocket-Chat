using AdminPanel.WebUI.Models;
using System.Collections.Concurrent;

namespace AdminPanel.WebUI.WebSocketHandlers.Abstract
{
    public interface ICallBackBinder
    {
        void Bind(ConcurrentDictionary<string, BindData> binds);
    }
}