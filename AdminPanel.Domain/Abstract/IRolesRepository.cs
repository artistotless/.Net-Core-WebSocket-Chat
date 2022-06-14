using AdminPanel.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Abstract
{
    public interface IRolesRepository
    {
        IQueryable<Role> Roles { get; }
        Task AddAsync(Role role);
        Task DeleteAsync(Role role);
        Task<Role> GetByTitleAsync(string title);
        Task<Role> GetByIDAsync(int id);
    }
}