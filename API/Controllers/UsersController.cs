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

    public UsersController(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        //var users = await _repository.GetUsersAsync();
        //var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        //return Ok(usersToReturn);
        return Ok(await _repository.GetMembersAsync());
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
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var user = await _repository.GetUserByUserNameAsync(username);
      if (user==null) return NotFound();
      
       _mapper.Map(memberUpdateDto, user);
      if(await _repository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to update user");
    }
}
