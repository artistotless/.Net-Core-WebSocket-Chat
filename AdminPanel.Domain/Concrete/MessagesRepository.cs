using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Domain.Concrete
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly EFDbContext _dbContext;

        public MessagesRepository(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Message> Messages
        {
            get
            {
                return _dbContext.Messages
                .Include(x => x.Operator)
                .Include(x => x.Player);
            }
        }

        public async Task<int> CountAsync() => _dbContext.Messages.Count();

        public async Task<Message> AddAsync(Message message)
        {
            var addedMessage = await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return addedMessage.Entity;
        }

        public async Task<IEnumerable<Message>> GetByIdsAsync(int operatorId, int playerId)
        {
            return await Messages.Where(x =>
            x.Operator.OperatorID == operatorId && x.Player.PlayerID == playerId)
            .ToListAsync();
        }

        public async Task<Message> RemoveAsync(int messageId)
        {
            Message entry = await _dbContext.Messages.FirstOrDefaultAsync(x => x.MessageID == messageId);
            if (entry != null)
            {
                _dbContext.Messages.Remove(entry);
                await _dbContext.SaveChangesAsync();
            }
            return entry;
        }

        public async Task SetMessageRead(int messageId)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(x => x.MessageID == messageId);
            if (message == null)
            {
                throw new DbUpdateException();
            }
            message.Read = true;
            await _dbContext.SaveChangesAsync();
        }

    }
}
