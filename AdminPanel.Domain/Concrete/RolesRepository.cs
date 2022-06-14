using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Concrete
{
    public class RolesRepository : IRolesRepository
    {
        private readonly EFDbContext _dbContext;

        public RolesRepository(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Role> Roles
        {
            get
            {
                return _dbContext.Roles
                .Include(x => x.Permission);
            }
        }

        public async Task AddAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Role> GetByIDAsync(int id)
        {
            return await Roles.FirstOrDefaultAsync(x => x.RoleID == id);
        }

        public async Task<Role> GetByTitleAsync(string title)
        {
            return await Roles.FirstOrDefaultAsync(x => x.Title == title);
        }
    }
}
