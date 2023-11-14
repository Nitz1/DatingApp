using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize]
public class MessageHub:Hub
{
    public IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
                      IMapper mapper,IHubContext<PresenceHub> presenceHub)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _presenceHub = presenceHub;
    }
    public async override Task OnConnectedAsync()
    {
   
        var httpContext = Context.GetHttpContext();
        var otheruser = httpContext.Request.Query["user"];
        var groupName= GetGroupName(Context.User.GetUsername(),otheruser);

        await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
        var group = await AddTogroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = await _messageRepository.GetMesageThread(Context.User.GetUsername(),otheruser);

        //await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages);
        await Clients.Caller.SendAsync("ReceiveMessageThread",messages);

        //return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }

    public string GetGroupName(string caller, string other)
    {
        var stringCompare = String.CompareOrdinal(caller,other)<0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    public async Task SendMessage(CreateMessageDto messageDto)
    {
        var username = Context.User.GetUsername();

        if(username==messageDto.RecepientUsername) 
            throw new HubException("You can not send messages to yourself");
        
        var senderId = Context.User.GetUserId();
        var recipient = await _userRepository.GetUserByUserNameAsync(messageDto.RecepientUsername);
        var sender = await _userRepository.GetUserByIdAsync(senderId);
       // var recipient = _userRepository.GetUserByIdAsync(recepientId)
        if (recipient == null) throw new HubException("Not found user");
        Message message = new Message(){
            Sender = sender,
            Recepient = recipient,
            SenderUsername = sender.UserName,
            RecepientUsername = recipient.UserName,
            Content = messageDto.Content
        };

        var groupName= GetGroupName(sender.UserName,recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        if(group.Connections.Any(x=>x.Username==recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if(connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new {username = sender.UserName, knownAs=sender.KnownAs});

            }
        }
        _messageRepository.AddMessage(message);

        if(await _messageRepository.SaveAllAsync())
        {
            //var groupName= GetGroupName(Context.User.GetUsername(),recipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage",_mapper.Map<MessageDto>(message));
        }
    }

    //public async Task<bool> AddTogroup(string groupName)
    public async Task<Group> AddTogroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);

        var connection = new Connection(Context.ConnectionId,Context.User.GetUsername());

        if(group==null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);

        }

        group.Connections.Add(connection);

        if( await _messageRepository.SaveAllAsync()) return group;

        throw new HubException("Failed to add group cannot");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        //var connection =await _messageRepository.GetConnection(Context.ConnectionId);
        var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);

        _messageRepository.RemoveConnection(connection);

        if ( await _messageRepository.SaveAllAsync()) return group;
         throw new HubException("Failed to remove from group");

    }
}
