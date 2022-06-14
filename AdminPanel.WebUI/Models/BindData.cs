using System;
using System.Threading.Tasks;

namespace AdminPanel.WebUI.Models
{
    public struct BindData
    {
        public Func<dynamic, Task> callback { get; set; }
        public dynamic sheme { get; set; }
    }
}
