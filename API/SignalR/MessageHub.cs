using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using API.DataTransferObjects;
using API.Entities;

namespace API.SignalR
{
  /* This class implements the Hub class of SignalR.
     It gives users the ability to live chat with
     each other. They no longer have to refresh the
     page to check if they got any new messages. */
  public class MessageHub : Hub
  {
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public MessageHub(IMapper mapper,
      IMessageRepository messageRepository, IUserRepository userRepository)
    {
      _messageRepository = messageRepository;
      _userRepository = userRepository;
      _mapper = mapper;
    }
    public override async Task OnConnectedAsync()
    {
      /* Create a signalR group for each pair of messaging users.
         We define the group name as combination of caller_username
         + receiver_username in alphabetical order. For example:
         The group name is LisaTodd and Todd joins the group.
         The group name should still remain as LisaTodd so that
         Todd or Lisa joins the same group every time they
         connect to this hub. The caller is the user who messages
         the reciever.
         . */

      var httpContext = Context.GetHttpContext();

      //change usernames to titlecase for easier readability
      var caller = CultureInfo.CurrentCulture
        .TextInfo
        .ToTitleCase(Context.User.GetUsername());

      /* When a connection is made to this hub, we get the receiver's
         username using a query string, which is a key of 'user'.
         This is how we know which user profile, the currently
         logged in user has clicked on.
      */
      var reciever = CultureInfo.CurrentCulture
        .TextInfo
        .ToTitleCase(httpContext.Request.Query["user"].ToString());

      //define the group name and add it to the hub connection
      var groupName = GetGroupName(caller, reciever);
      await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

      var messages = await _messageRepository
        .GetMessageThread(caller, reciever);

      await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      await base.OnDisconnectedAsync(exception);
    }

    /* A helper method that combines two strings
       in alphabetical order. */
    private string GetGroupName(string caller, string reciever)
    {
      var stringCompare = string.CompareOrdinal(caller, reciever) < 0;

      return stringCompare ? $"{caller}-{reciever}" : $"{reciever}-{caller}";
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
      var username = Context.User.GetUsername();

      if(username == createMessageDTO.RecipientUsername.ToLower())
        throw new HubException("You cannot send messages to yourself");

      var sender = await _userRepository.GetUserByUsernameAsync(username);
      var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

      if(recipient == null)
        new HubException("NotFound");

      var message = new Message
      {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDTO.Content,
      };

      _messageRepository.AddMessage(message);

      if(await _messageRepository.SaveAllAsync())
      {
        var group = GetGroupName(sender.UserName, recipient.UserName);
        await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
      }


      new HubException("Failed to send message");

    }
  }
}
