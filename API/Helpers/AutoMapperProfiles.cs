using API.Entities;
using AutoMapper;

namespace API;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {       
        CreateMap<AppUser, MemberDto>().
            ForMember(dest=>dest.PhotoUrl, opt=>opt.
            MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain==true).Url)).
            ForMember(dest=>dest.Age, opt=>opt.
            MapFrom(src=>src.DateOfBirth.CalculateAge()));

        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();/*.
        ForMember(dest=>dest.City, opt=>opt.
        MapFrom(src=>src.City)).
        ForMember(dest=>dest.Interests, opt=>opt.
        MapFrom(src=>src.Interests)).
        ForMember(dest=>dest.Country, opt=>opt.
        MapFrom(src=>src.Country)).
        ForMember(dest=>dest.LookingFor, opt=>opt.
        MapFrom(src=>src.LookingFor)).
         ForMember(dest=>dest.Introduction, opt=>opt.
        MapFrom(src=>src.Introduction)).
        ForMember(dest => dest.DateOfBirth, act => act.Ignore()).
        ForMember(dest => dest.KnownAs, act => act.Ignore()).
        ForMember(dest => dest.Created, act => act.Ignore()).
        ForMember(dest => dest.LastActive, act => act.Ignore()).
        ForMember(dest => dest.Gender, act => act.Ignore()).
        ForMember(dest => dest.Photos, act => act.Ignore()).
        ForMember(dest => dest.Id, act => act.Ignore()).
        ForMember(dest => dest.UserName, act => act.Ignore()).
        ForMember(dest => dest.PasswordHash, act => act.Ignore()).
        ForMember(dest => dest.PasswordSalt, act => act.Ignore());*/
    }
}
