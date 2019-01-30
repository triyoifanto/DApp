using System.Linq;
using AutoMapper;
using DatingApp.API.Dto;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        // automapper is convention based so it will be refer to the property name, if no configuration defined on mapper
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDto>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });

            CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });
            
            CreateMap<Photo, PhotoDetailDto>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Photo, PhotoRetunDto>();
            CreateMap<ImagePhotoDto, Photo>();
        }
    }
}