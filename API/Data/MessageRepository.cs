using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataTransferObjects;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class MessageRepository : IMessageRepository
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }
    public void AddMessage(Message message)
    {
      _context.Message.Add(message);
    }

    public  void DeleteMessage(Message message)
    {
      _context.Message.Remove(message);
    }
    public async Task<Message> GetMessage(int id)
    {
      return await _context.Message
        .Include(u => u.Sender)
        .Include(u => u.Recipient)
        .SingleOrDefaultAsync(x => x.Id == id);
    }
    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParameters messageParams)
    {
     var query = _context.Message
      .OrderByDescending(m => m.MessageSent)
      .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
      .AsQueryable();

      query = messageParams.Container switch
      {
        "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
          && u.RecipientDeleted == false),
        "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
          && u.SenderDeleted == false),
        _ => query.Where(u => u.RecipientUsername ==
            messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
      };

      return await PagedList<MessageDTO>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
      //we get the conversation of the users
      var messages = await _context.Message
        .Where(m => m.Recipient.UserName == currentUsername
            && m.RecipientDeleted == false
            && m.Sender.UserName == recipientUsername
            || m.Recipient.UserName == recipientUsername
            && m.Sender.UserName == currentUsername
            && m.SenderDeleted == false
        )
        .OrderBy(m => m.MessageSent)
        .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
        .ToListAsync();

        /*find out if there's any unread messages for the current user
          that they have received */
        var unreadMessages = messages.Where(m => m.DateRead == null
          && m.RecipientUsername == currentUsername).ToList();

        //we mark the messages as read
        if(unreadMessages.Any())
        {
          foreach (var message in unreadMessages)
          {
            message.DateRead = DateTime.UtcNow;
          }
        }

        //return message DTOs
        return messages;
    }
    public void AddGroup(Group group)
    {
      _context.Groups.Add(group);
    }

    /* Accepts a connectionId and returns the group that has that connectionID.
       Each connection has a corresponding group name foreign key.*/
    public async Task<Group> GetGroupForConnection(string connectionId)
    {
      return await _context.Groups
        .Include(c => c.Connections)
        .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }
    public async Task<Group> GetMessageGroup(string groupName)
    {
      return await _context.Groups
      .Include(x => x.Connections)
      .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
      return await _context.Connections.FindAsync(connectionId);
    }


    public void RemoveConnection(Connection connection)
    {
     _context.Connections.Remove(connection);
    }

  }
}
