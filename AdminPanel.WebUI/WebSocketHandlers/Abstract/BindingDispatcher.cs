
using AdminPanel.WebUI.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.WebSocketHandlers.Abstract
{
    public abstract class BindingDispatcher
    {
        public abstract ConcurrentDictionary<string, BindData> BindCallbacks { get; set; }
        private bool isBinded = false;
        public void ApplyBindings(ICallBackBinder binder)
        {
            if (!isBinded)
            {
                binder.Bind(BindCallbacks);
                isBinded = true;
            }
        }

        public async Task Handle(string json)
        {
            var typeSheme = new { type = "" };
            var objectWithoutData = JsonConvert.DeserializeAnonymousType(json, typeSheme);
            BindData bindData;
            BindCallbacks.TryGetValue(objectWithoutData.type, out bindData);
            var result = JsonConvert.DeserializeAnonymousType(json, bindData.sheme);
            await bindData.callback?.Invoke(result.data);
        }
    }
}
