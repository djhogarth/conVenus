namespace API.SignalR
{
    [Authorize]
    public class PresenceHub: Hub
    {
    private readonly PresenceTracker _tracker;

    public PresenceHub(PresenceTracker tracker)
    {
      _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
      {
        var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

        /* If the user is online,then send the 'UserisOnline' to all other connected users */
        if(isOnline)
          await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
      }

      public override async Task OnDisconnectedAsync(Exception exception)
      {
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        /* If user is offline, then notify other users */
        if(isOffline)
          await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        await base.OnDisconnectedAsync(exception);
      }
    }
}
