namespace API.SignalR
{
    public class PresenceTracker
    {
        /* For now, a dictionary will be a shared resource for the purpose of tracking all online users.
           However, dictionaries are not thread-safe. So if concurrent users log in or out, issues will arise.
           To solve this isssue, we implement mutual exclusion by adding a lock to the dictionary,
           thus preventing race conditions*/

        /* Online takes the user's username as the key and a list
           of their connectionIds as the value */
        private readonly Dictionary<string, List<string>> OnlineUsers =
         new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId)
        {
          /* isOnline is only true if a user comes online with no other devices connected  */
          bool isOnline = false;
          lock(OnlineUsers)
          {
            /* Check if a dictionary element exists with key of the
               currently logged in username. If THERE IS then we
               add the connectionId, otherwise we create a new
               dictionary entry for the particular username and add
               the connectionId */
            if(OnlineUsers.ContainsKey(username))
            {
              OnlineUsers[username].Add(connectionId);
            }
            else
            {
              OnlineUsers.Add(username, new List<string>{connectionId});
              isOnline = true;
            }
          }

          return Task.FromResult(isOnline);
        }
        public Task<bool> UserDisconnected(string username, string connectionId)
        {

          /* isOffline is only true when a user has no other
             devices logged into their user account */
          bool isOffline = false;

          lock(OnlineUsers)
          {
            /* Check if a dictionary element already exists
               with the key of the currently logged in username.
               If none exists, then the task is done, unlock critical
               section. If THERE IS such an element, we remove the
               connectionId from the element. If a element
               has 0 connectionIds for said username, it's removed. */

            if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
              OnlineUsers.Remove(username);
              isOffline = true;
            }
          }

          return Task.FromResult(isOffline);
        }

        //Gets a list of connectionIDs for a particular user
        public Task<List<string>> GetConnectionsForUser(string username)
        {
          List<string> connectionIds;

          lock(OnlineUsers)
          {
            connectionIds = OnlineUsers.GetValueOrDefault(username);
          }

          return Task.FromResult(connectionIds);
        }

        /* Returns a string of all of users currently
           logged-in, ordered by the username. */
        public Task<string[]> GetOnlineUsers()
        {
          string[] onlineUsers;
          lock(OnlineUsers)
          {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
          }

          return Task.FromResult(onlineUsers);
        }

    }
}
