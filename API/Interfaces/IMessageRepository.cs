namespace API;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMesagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMesageThread (string currentUserName, string recipientUserName);
    // Task<bool> SaveAllAsync(); 

    void AddConnection(Connection connection);
    void RemoveConnection(Connection connection);
    Task<Group> GetMessageGroup(string groupName);
    void AddGroup(Group group);
    Task<Connection> GetConnection(string connectionId);

    Task<Group> GetGroupForConnection(string connectionId);
}
