using AdminPanel.Domain.Concrete;
using AdminPanel.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Abstract
{
    public interface IMessagesRepository
    {
        IQueryable<Message> Messages { get; }
        Task<int> CountAsync();
        Task<Message> AddAsync(Message message);
        Task<Message> RemoveAsync(int messageId);
        Task SetMessageRead(int messageId);
        Task<IEnumerable<Message>> GetByIdsAsync(int operatorId, int playerId);

    }
}