
using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddConnection(Connection connection)
    {
        _context.Connections.Add(connection);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
       return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups.Include(x=>x.Connections)
               .Where(x=>x.Connections.Any(c=>c.ConnectionId == connectionId))
               .FirstOrDefaultAsync();
    }

    public async Task<PagedList<MessageDto>> GetMesagesForUser(MessageParams messageParams)
    {
        var query =  _context.Messages.OrderByDescending(x=>x.MessageSent)
                    .AsQueryable();
        query = messageParams.Container switch
                  {
                     "Inbox" =>query.Where(x=>x.RecepientUsername == messageParams.UserName && x.RecepientDeleted==false),
                     "Outbox" => query.Where(x=>x.SenderUsername == messageParams.UserName  && x.SenderDeleted==false),
                     _ => query.Where(x=>x.RecepientUsername==messageParams.UserName && x.DateRead==null && x.RecepientDeleted==false)
                  };
      var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

      return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
                
    }

    public async Task<IEnumerable<MessageDto>> GetMesageThread(string currentUserName, string recipientUserName)
    {
       var messages =await _context.Messages.Include(x=>x.Sender).
                  ThenInclude(x=>x.Photos).Include(x=>x.Recepient).
                  ThenInclude(x=>x.Photos).Where(x=>
                         x.RecepientUsername== currentUserName && 
                         x.SenderUsername == recipientUserName && x.RecepientDeleted == false
                     ||  x.SenderUsername == currentUserName && 
                     x.RecepientUsername == recipientUserName && x.SenderDeleted == false
                  ).OrderByDescending(x=>x.MessageSent).ToListAsync();

        var unreadMessages = messages.Where(x=>x.DateRead==null && x.RecepientUsername==currentUserName)
                             .ToList();
        
        if(unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

       return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups.Include(x=>x.Connections)
                     .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }

    void IMessageRepository.AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }
}
