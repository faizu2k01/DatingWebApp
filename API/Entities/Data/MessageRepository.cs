using API.Entities.Dto;
using API.Helper;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
             _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _dataContext.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _dataContext.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Message.Remove(message);
        }

        public async Task<Group> GetGroup(string groupName)
        {
            return await _dataContext.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Message
                .Include(u => u.Sender)
                .Include(u=> u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Message.OrderByDescending(m => m.MessageSent).ProjectTo<MessageDto>(_mapper.ConfigurationProvider).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbo" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Message
                .Where(m => m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false
                || m.Recipient.UserName == recipientUsername && m.SenderDeleted == false && m.Sender.UserName == currentUsername)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .OrderBy(m => m.MessageSent).ToListAsync();

            var unreadedMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            if (unreadedMessages.Any())
            {
                foreach(var message in unreadedMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            await _dataContext.SaveChangesAsync();

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _dataContext.Connections.Remove(connection);
        }

       public async Task<Connection> GetConnection(string connectionId)
        {
            return await _dataContext.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _dataContext.Groups
                 .Include(c => c.Connections)
                 .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                 .FirstOrDefaultAsync();
        }
    }
}
