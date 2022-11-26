using API.Entities;
using API.Entities.Dto;
using API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.interfaces
{
    public  interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Group> GetGroup(string groupName);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetGroupForConnection(string connectionId);

        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

    }
}
