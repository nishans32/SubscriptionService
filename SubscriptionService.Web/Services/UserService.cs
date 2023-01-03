using AutoMapper;
using SubscriptionService.Web.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Repositories;
using SubscriptionService.Web.Validators;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Services
{
    public interface IUserService
    {
        Task<GetUserResponse> GetUser(Guid userid);
        Task<IEnumerable<GetUserResponse>> ListUsers();
        Task CreateUser(CreateUserRequest userDto);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }
        public async Task<GetUserResponse> GetUser(Guid userid)
        {
            var user = await _userRepo.GetUser(userid);
            return _mapper.Map<GetUserResponse>(user);
        }

        public async Task<IEnumerable<GetUserResponse>> ListUsers()
        {
            var users = await _userRepo.GetAllUsers();
            return users.Select(user => _mapper.Map<GetUserResponse>(user));
        }

        public async Task CreateUser(CreateUserRequest userDto)
        {
            userDto.Validate();

            if (await IsEmailAddressAlreadyRegistered(userDto.Email))
                throw new ValidationException("Email address already used to create a user");

            var user = _mapper.Map<User>(userDto);
            await _userRepo.InsertUser(user);
        }

        private async Task<bool> IsEmailAddressAlreadyRegistered(string emailAddress)
        {
            var user = await _userRepo.GetUserByEmail(emailAddress);
            return user != null;
        }

    }

}