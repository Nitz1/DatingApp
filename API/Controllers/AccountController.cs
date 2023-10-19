using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
   {
        _context = context;
        _tokenService = tokenService;        
        _mapper = mapper;
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await UserExists(registerDto.Username)) return BadRequest("username already taken");
        using var hmac = new HMACSHA512();
        var user = _mapper.Map<AppUser>(registerDto);

        /*var user = new AppUser()
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        }; */

        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto(){
            Username = registerDto.Username,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
        };
    }

    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
    }

[HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user =await _context.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.UserName == loginDto.Username.ToLower());

        if(user == null) return Unauthorized();

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i=0;i< computedHash.Length;i++ )
        {
            if(user.PasswordHash[i] != computedHash[i])
            {
                return Unauthorized("Invalid Password");
            }
        }
        return new UserDto(){
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
        };
    }
}
