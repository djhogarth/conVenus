using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace API.SignalR
{
  /* This class implements the Hub class of SignalR.
     It gives users the ability to live chat with
     each other. They no longer have to refresh the
     page to check if they got any new messages. */
  public class MessageHub : Hub
  {
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageHub(IMessageRepository messageRepository, IMapper mapper)
    {
      _messageRepository = messageRepository;
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
  }
}
