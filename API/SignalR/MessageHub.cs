namespace API.SignalR
{
  /* This class implements the Hub class of SignalR.
     It gives users the ability to live chat with
     each other. They no longer have to refresh the
     page to check if they got any new messages. */
  public class MessageHub : Hub
  {
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MessageHub (IHubContext<PresenceHub> presenceHub, PresenceTracker tracker,
      IMapper mapper, IUnitOfWork unitOfWork)
    {
      _presenceHub = presenceHub;
      _tracker = tracker;
      _mapper = mapper;
      _unitOfWork = unitOfWork;
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

      var caller = Context.User.GetUsername();

      /* When a connection is made to this hub, we get the receiver's
         username using a query string, which is a key of 'user'.
         This is how we know which user profile, the currently
         logged in user has clicked on.
      */
      var reciever = httpContext.Request.Query["user"].ToString();

      //Define the group name and add it to the hub connection.
      var groupName = GetGroupName(caller, reciever);
      await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
      var group = await AddToGroup(groupName);
      await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

      /* Get the message thread between the caller and receiver
         and save any changes to the database if there are any. */
      var messages = await _unitOfWork.MessageRepository
        .GetMessageThread(caller, reciever);

      if(_unitOfWork.HasChanges())  await _unitOfWork.Complete();

      //Send the ReceiveMessageThread method to whoever is connecting
      await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      var group = await RemoveFromMessageGroup();
      await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
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

      var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

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

      var groupName = GetGroupName(sender.UserName, recipient.UserName);

      var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

      /* Checks if the two users are connected to the same chat window
         or in the same group */
      if(group.Connections.Any(x => x.Username == recipient.UserName))
      {
        message.DateRead = DateTime.UtcNow;
      } else {

        /* If the receiver/recipient is online but not
           connected to the same group as the current user */
        var connections = await _tracker.GetConnectionsForUser(recipient.UserName);

        if(connections != null)
        {
          await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
            new {username = sender.UserName, alias = sender.Alias });
        }
      }

      _unitOfWork.MessageRepository.AddMessage(message);

      if(await _unitOfWork.Complete())
      {
        await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
      }
    }

    /* Takes in a group name and returns a new or existing
       group with updated conenctions */
    private async Task<Group> AddToGroup(string groupName)
    {
      //Get the group by the group and create a new connection
      var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
      var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

      /* If a group by that name does not exist yet,
         great a new group */

      if(group == null)
      {
        group = new Group(groupName);
        _unitOfWork.MessageRepository.AddGroup(group);
      }

      group.Connections.Add(connection);

      /* Check if adding the group to the database was successful */
      if( await _unitOfWork.Complete()) return group;

      throw new HubException("Failed to add the message group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
      var connectionId = Context.ConnectionId;
      var group = await _unitOfWork.MessageRepository.GetGroupForConnection(connectionId);
      var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == connectionId);

      _unitOfWork.MessageRepository.RemoveConnection(connection);

      /* Check if removing the connection from the group was successful*/
      if(await _unitOfWork.Complete())
        return group;

      throw new HubException("Failed to remove connection from group");
    }
  }
}
