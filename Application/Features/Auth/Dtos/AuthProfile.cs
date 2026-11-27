using Application.Features.Auth.Commands.CompleteProfile;
using Application.Features.Auth.Commands.UpdateCurrentUser;

namespace Application.Features.Auth.Dtos
{
    public sealed class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.BirthDate));

            CreateMap<Passenger, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            //CreateMap<Captaion, UserDto>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<UpdateCurrentUserCommand, ApplicationUser>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth));

            CreateMap<CompleteProfileCommand, ApplicationUser>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth));
        }
    }
}