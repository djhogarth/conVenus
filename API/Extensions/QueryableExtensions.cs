namespace API.Extensions
{
  public static class QueryableExtensions
  {
    public static IQueryable<Message> MarkUnreadAsRead(this IQueryable<Message> query, string currentUsername)
    {
      // Get a list of the unread messages for current user.
      var unreadMessages = query.Where(m => m.DateRead == null
          && m.RecipientUsername == currentUsername);

      // Set any unread messages to read with current date time.
      if (unreadMessages.Any())
      {
          foreach (var message in unreadMessages)
          {
              message.DateRead = DateTime.UtcNow;
          }
      }

      return query;
    }

  }
}
