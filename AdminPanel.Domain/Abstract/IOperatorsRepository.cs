using AdminPanel.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Abstract
{
    public interface IOperatorsRepository
    {
        IQueryable<Operator> Operators { get; }
        Task AddAsync(Operator @operator);
        Task<Operator> GetByNameAsync(string loginName);
        Task<Operator> GetByEmailAsync(string email);
    }
}