using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    //private readonly IUserRepository _userRepository;
    //private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    // public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, 
    // IMapper mapper)
    // {
    //     _userRepository = userRepository;
    //     _messageRepository = messageRepository;
    //     _mapper = mapper;
    // }

        public MessagesController( IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ActionResult> AddMessage(CreateMessageDto messageDto)
    {
        var senderId = User.GetUserId();
        // var recipient = await _userRepository.GetUserByUserNameAsync(messageDto.RecepientUsername);
        // var sender = await _userRepository.GetUserByIdAsync(senderId);
        var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(messageDto.RecepientUsername);
        var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(senderId);
       // var recipient = _userRepository.GetUserByIdAsync(recepientId)
        if (recipient == null) return NotFound();
        Message message = new Message(){
            Sender = sender,
            Recepient = recipient,
            SenderUsername = sender.UserName,
            RecepientUsername = recipient.UserName,
            Content = messageDto.Content
        };

        //_messageRepository.AddMessage(message);
        _unitOfWork.MessageRepository.AddMessage(message);

        //if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
        if(await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed tosend message");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDto>>> GetMesagesForUser( [FromQuery] 
    MessageParams messageParams)
    {
        messageParams.UserName = User.GetUsername();
      //var messages =  await _messageRepository.GetMesagesForUser(messageParams);
      var messages =  await _unitOfWork.MessageRepository.GetMesagesForUser(messageParams);
      Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
      messages.PageSize, messages.TotalCount, messages.TotalPages));
      return messages;
    }

    // [HttpGet("thread/{username}")]
    // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    // {
    //     string currentUserName = User.GetUsername();
        
    //     // return Ok( await _messageRepository.GetMesageThread(currentUserName,username));
    //      return Ok( await _unitOfWork.MessageRepository.GetMesageThread(currentUserName,username));
    // }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        //var message =await _messageRepository.GetMessage(id);
         var message =await _unitOfWork.MessageRepository.GetMessage(id);

        if(message.RecepientUsername != username && message.SenderUsername != username) return Unauthorized();

        if(message.SenderUsername == username) message.SenderDeleted = true;
        if(message.RecepientUsername == username) message.RecepientDeleted = true;

        if(message.SenderDeleted==true && message.RecepientDeleted==true)
        {
            //_messageRepository.DeleteMessage(message);
            _unitOfWork.MessageRepository.DeleteMessage(message);
        }

        //if(await _messageRepository.SaveAllAsync()) return Ok();
        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting message");
    }
}
