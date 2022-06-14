using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Concrete
{
    public class OperatorsRepository : IOperatorsRepository
    {
        private readonly EFDbContext _dbContext;

        public OperatorsRepository(EFDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<Operator> Operators
        {
            get
            {
                return _dbContext.Operators
                .Include(x => x.Role)
                .Include(x => x.Role.Permission);
            }
        }

        public async Task AddAsync(Operator @operator)
        {
            await _dbContext.Operators.AddAsync(@operator);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Operator> RemoveAsync(int operatorId)
        {
            Operator entry = _dbContext.Operators.FirstOrDefault(x => x.OperatorID == operatorId);
            if (entry != null)
            {
                _dbContext.Operators.Remove(entry);
                await _dbContext.SaveChangesAsync();
            }
            return entry;
        }

        public async Task<Operator> GetByEmailAsync(string email)
        {
            return await Operators.FirstOrDefaultAsync(o => o.Email == email);
        }

        public async Task<Operator> GetByNameAsync(string loginName)
        {
            return await Operators.FirstOrDefaultAsync(o => o.Name == loginName);
        }
    }
}
