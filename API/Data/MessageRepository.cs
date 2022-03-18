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
      return await _context.Message.FindAsync(id);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParameters messageParams)
    {
     var query = _context.Message
      .OrderByDescending(m => m.MessageSent)
      .AsQueryable();

      query = messageParams.Container switch
      {
        "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
        "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
        _ => query.Where(u => u.Recipient.UserName ==
            messageParams.Username && u.DateRead == null)
      };

      var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

      return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
      //we get the conversation of the users
      var messages = await _context.Message
        .Include(u => u.Sender).ThenInclude(p => p.Photos)
        .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        .Where(m => m.Recipient.UserName == currentUsername
            && m.Sender.UserName == recipientUsername
            || m.Recipient.UserName == recipientUsername
            && m.Sender.UserName == currentUsername
        )
        .OrderBy(m => m.MessageSent)
        .ToListAsync();

        /*find out if there's any unread messages for the current user
          that they have received */
        var unreadMessages = messages.Where(m => m.DateRead == null
          && m.Recipient.UserName == currentUsername).ToList();

        //we mark the messages as read
        if(unreadMessages.Any())
        {
          foreach (var message in unreadMessages)
          {
            message.DateRead = DateTime.Now;
          }

          await _context.SaveChangesAsync();
        }

        //return message DTOs
        return _mapper.Map<IEnumerable<MessageDTO>>(messages);

    }

    public async Task<bool> SaveAllAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }
  }
}
