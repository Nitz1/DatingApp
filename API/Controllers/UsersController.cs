using System.Security.Claims;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
    {
        _repository = repository;
        _mapper = mapper;
        _photoService = photoService;
    }

    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    // {
    //     return Ok(await _repository.GetUsersAsync());
    // }

    // [HttpGet("{id}")]
  
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     return await _context.Users.FindAsync(id);
    // }

    // [HttpGet("{username}")]
    // public async Task<ActionResult<AppUser>> GetUser(string username)
    // {
    //     return await _repository.GetUserByUserNameAsync(username);
    // }

      [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
      var user= await _repository.GetUserByUserNameAsync(User.GetUsername());
          if(string.IsNullOrEmpty(userParams.Gender)){
            userParams.Gender = user.Gender=="male"? "female" : "male";
          }
          
          userParams.CurrentUsername = user.UserName;
      var users = await _repository.GetMembersAsync(userParams);
      Response.AddPaginationHeader(new PaginationHeader(userParams.PageNumber,
          userParams.PageSize,users.TotalCount,users.TotalPages));
        //var users = await _repository.GetUsersAsync();
        //var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        //return Ok(usersToReturn);
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
       // var user = await _repository.GetUserByUserNameAsync(username);
       // return _mapper.Map<MemberDto>(user);
       return await _repository.GetMemberAsyc(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
      var user = await _repository.GetUserByUserNameAsync(User.GetUsername());
      if (user==null) return NotFound();
      
       _mapper.Map(memberUpdateDto, user);
      if(await _repository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]

    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await _repository.GetUserByUserNameAsync(User.GetUsername());
      if(user==null) return NotFound("User not found");
       var result = await _photoService.AddPhotoAsync(file);

       if(result.Error != null ) return BadRequest(result.Error.Message);

       Photo photo = new Photo{
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
       };

       if(user.Photos.Count()==0)  photo.IsMain = true;
       user.Photos.Add(photo);
       if(await _repository.SaveAllAsync()) 
       {
        return CreatedAtAction(nameof(GetUsers),new {username = user.UserName},
        _mapper.Map<PhotoDto>(photo));
       }
       

       return BadRequest("Problem adding photo");
    }
    
    [HttpPut("set-main-photo/{photoId}")]

    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      var user = await _repository.GetUserByUserNameAsync(User.GetUsername());

      if(user == null) return NotFound();

      var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);

      if(photo==null) return NotFound();

      if(photo.IsMain) return BadRequest("this is already your main photo");

      var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;

      photo.IsMain = true;
      if(await _repository.SaveAllAsync()) return NoContent();

      return BadRequest("Error while setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult> DeletePhoto(int photoId){
      var user = await _repository.GetUserByUserNameAsync(User.GetUsername());
      if(user == null) return NotFound();

      var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);
      if(photo == null) return NotFound();

      if(photo.IsMain) return BadRequest("Main photo cannot be deleted");

      if(photo.PublicId != null){
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);
         if(result.Error != null) return BadRequest(result.Error.ToString());
      }
      user.Photos.Remove(photo);
      if(await _repository.SaveAllAsync()) return Ok();

      return BadRequest("Problem deleting photo");
    }
}

