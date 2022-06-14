using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Concrete
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly EFDbContext _dbContext;

        public PlayersRepository(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Player> Players => _dbContext.Players;

        public async Task AddAsync(Player player)
        {
            await _dbContext.Players.AddAsync(player);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Player> GetByTokenAsync(string token)
        {
            Player player = null;
            string encodedToken = System.Web.HttpUtility.UrlEncode(token, System.Text.Encoding.UTF8);
            player = await Players.FirstOrDefaultAsync(x => x.Token == encodedToken);
            return player;
        }
    }
}
