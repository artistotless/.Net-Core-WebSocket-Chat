using AdminPanel.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Abstract
{
    public interface IPlayersRepository
    {
        IQueryable<Player> Players { get; }
        Task AddAsync(Player player);
        Task<Player> GetByTokenAsync(string token);
    }
}