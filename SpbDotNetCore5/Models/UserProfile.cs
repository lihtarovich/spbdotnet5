using AutoMapper;
using DataAccessLayer;

namespace SpbDotNetCore5.Models
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DbUser, DtoUser>().ReverseMap();
            CreateMap<DbPhoneNumber, DtoPhoneNumber>().ForMember(
                    dest => dest.Phone ,
                    opt => opt.MapFrom(src => src.PhoneNumber)) 
                .ReverseMap();
        }
    }
}