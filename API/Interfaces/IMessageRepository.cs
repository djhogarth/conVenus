using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParameters messageParams);

        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();

    }
}
