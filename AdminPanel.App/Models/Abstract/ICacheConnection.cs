
namespace AdminPanel.App.Models.Abstract
{
    public interface ICacheConnection
    {
        /// <param name="connectionString">IP:Port Password (optional)</param>
        ICacheConnection Connect(string connectionString);
    }
}
