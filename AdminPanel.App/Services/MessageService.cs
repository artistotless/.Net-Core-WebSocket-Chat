using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.App.Services.Abstract;
using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdminPanel.App.Services
{
    public class MessageService
    {
        private const int PAGE_SIZE = 10;
        private readonly IMessagesRepository _messagesRepository;
        private readonly ICacheService _cache;

        public MessageService(IMessagesRepository messagesRepository, ICacheService cache)
        {
            _messagesRepository = messagesRepository;
            _cache = cache;
        }

        public async Task<Response<Message>> AddAsync(Message msg)
        {
            var response = new Response<Message>();
            try
            {
                response.Result = await _messagesRepository.AddAsync(msg);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Message>> AddAsync(string message, int playerId, int? operatorId = null, bool read = false)
        {
            var response = new Response<Message>();
            try
            {
                var newMessage = new Message()
                {
                    Text = message,
                    TimeStamp = DateTime.Now,
                    Read = read,
                    PlayerID = playerId,
                };

                if (operatorId.HasValue)
                    newMessage.OperatorID = operatorId.Value;

                response.Result = await _messagesRepository.AddAsync(newMessage);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<int>> CountAsync()
        {
            var response = new Response<int>();
            try
            {
                response.Result = await _messagesRepository.CountAsync();
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<int>> CountByPlayerAsync(int id)
        {
            var response = new Response<int>();
            try
            {
                if (id > 0)
                {
                    response.Result = await _messagesRepository.Messages.CountAsync(x => x.Player.PlayerID == id);
                }
                else
                {
                    response.Result = await _messagesRepository.CountAsync();
                }
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<int>> CountOfReadAsync()
        {
            var response = new Response<int>();
            try
            {
                response.Result = await _messagesRepository.Messages.CountAsync(x => x.Read == false);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<int>> CountOfUnreadAsync()
        {
            var response = new Response<int>();
            try
            {
                var cacheResponse = await _cache.GetValueAsync(nameof(CountOfUnreadAsync));

                if (cacheResponse.Result != null)
                {
                    response.Result = int.Parse(cacheResponse.Result);
                }
                else
                {
                    response.Result = await _messagesRepository.CountAsync();
                    await _cache.SetValueAsync(nameof(CountOfUnreadAsync), response.Result.ToString());
                }
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }

            return response;
        }

        public async Task<Response<IEnumerable<Message>>> GetDialogsAsync()
        {
            var response = new Response<IEnumerable<Message>>();
            try
            {
                response.Result = await Task.Run(() =>
                {
                    return _messagesRepository.Messages
                    .AsEnumerable()
                    .OrderByDescending(x=>x.MessageID)
                    .Distinct(new MessageComparer()).ToList();
                });
            }
            catch (Exception e)
            {
                Error.SetError(response, e.Message);
            }
            return response;
        }

        public async Task<Response<IEnumerable<Message>>> GetAllAsync(int page = 0)
        {
            return await GetAllWhereAsync(x => x != null, page);
        }

        public async Task<Response<IEnumerable<Message>>> GetAllByOperatorAsync(string name)
        {
            return await GetAllWhereAsync(x => x.Operator.Name == name);
        }

        public async Task<Response<IEnumerable<Message>>> GetAllByPlayerAsync(int id, int page = 0)
        {
            if (id > 0)
            {
                return await GetAllWhereAsync(x => x.Player.PlayerID == id, page);
            }
            else
            {
                return await GetAllAsync(page);
            }
        }

        public async Task<Response<IEnumerable<Message>>> GetAllByPlayerAsync(string name, int page = 0)
        {
            return await GetAllWhereAsync(x => x.Player.Name == name, page);
        }

        public async Task<Response<IEnumerable<Message>>> GetAllWhereAsync(Expression<Func<Message, bool>> predicate, int page = 0)
        {
            var response = new Response<IEnumerable<Message>>();
            try
            {
                var query = _messagesRepository.Messages.Where(predicate).OrderByDescending(x => x.MessageID);
                IQueryable<Message> pagingQuery = null;

                if (page != 0)
                    pagingQuery = query.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                response.Result = pagingQuery == null ?
                    await query.ToListAsync() :
                    await pagingQuery.ToListAsync();
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Message>> GetFirstWhereAsync(Expression<Func<Message, bool>> predicate)
        {
            var response = new Response<Message>();
            try
            {
                response.Result = await _messagesRepository.Messages
                .Where(predicate)
                .FirstOrDefaultAsync();
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<bool>> SetMessageRead(int messageId)
        {
            var response = new Response<bool>();
            try
            {
                await _messagesRepository.SetMessageRead(messageId);
                await _cache.DecrementValueAsync(nameof(CountOfUnreadAsync));
            }
            catch (DbUpdateException e)
            {
                Error.SetError(response, e.Message);
            }
            return response;
        }

    }
}
