using AutoMapper;
using System;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetUserResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalUserId));
            CreateMap<CreateUserRequest, User>()
                .ForMember(dest => dest.ExternalUserId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<Account, GetAccountResponse>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<UserAccount, GetUserAccountResponse>();
            CreateMap<CreateUserAccountRequest, Account>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
