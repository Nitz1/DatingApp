using API.Data;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberAsyc(string username)
    {
       return  await _context.Users.Where(x=>x.UserName==username).
             ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
             .SingleOrDefaultAsync();;
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
       // return _context.Users.
       //throw new Exception();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUserNameAsync(string name)
    {
        return await _context.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.UserName == name);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
       return await _context.Users.Include(x=>x.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
