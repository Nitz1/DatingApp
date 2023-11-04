using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;
public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;

    //private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)//DataContext context
   {
        _userManager = userManager;
        //_context = context;
        _tokenService = tokenService;        
        _mapper = mapper;
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await UserExists(registerDto.Username)) return BadRequest("username already taken");
        //using var hmac = new HMACSHA512();
        var user = _mapper.Map<AppUser>(registerDto);

        /*var user = new AppUser()
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        }; */

        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        // user.PasswordSalt = hmac.Key;
        var result = await _userManager.CreateAsync(user);
        if(!result.Succeeded) return BadRequest(result.Errors);

        var roleResults = await _userManager.AddToRoleAsync(user,"Member");

        if(!roleResults.Succeeded) return BadRequest(roleResults.Errors);
       // _context.Users.Add(user);
       // await _context.SaveChangesAsync();
        return new UserDto(){
            Username = registerDto.Username,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            Gender = user.Gender
        };
    }

    public async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x=>x.UserName==username.ToLower());
    }

[HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user =await _userManager.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.UserName == loginDto.Username.ToLower());

        if(user == null) return Unauthorized();

        //using var hmac = new HMACSHA512(user.PasswordSalt);

        //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        // for (int i=0;i< computedHash.Length;i++ )
        // {
        //     if(user.PasswordHash[i] != computedHash[i])
        //     {
        //         return Unauthorized("Invalid Password");
        //     }
        // }
        return new UserDto(){
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            Gender = user.Gender
        };
    }
}
