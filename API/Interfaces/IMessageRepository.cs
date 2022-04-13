namespace API.Interfaces
{
    public interface IMessageRepository
    {
        //Methods for general message function
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParameters messageParams);

        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);

        //Methods that help manage SignalR connections and groups.
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);

        Task<Group> GetGroupForConnection(string connectionId);
    }
}
