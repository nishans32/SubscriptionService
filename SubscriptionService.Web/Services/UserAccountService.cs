using AutoMapper;
using System;
using System.Threading.Tasks;
using SubscriptionService.Web.Models.DTO;
using SubscriptionService.Web.Repositories;
using System.Linq;
using System.Collections.Generic;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Validators;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Services
{
    public interface IUserAccountService
    {
        Task<GetUserAccountResponse> GetAccounts(Guid userId);
        Task CreateAccount(Guid userId, CreateUserAccountRequest userAccountDto);
    }

    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserAccountService(IUserAccountRepository userAccountRepo, IMapper mapper, IUserRepository userRepo)
        {
            _userAccountRepo = userAccountRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<GetUserAccountResponse> GetAccounts(Guid userId)
        {
            var userAccount = await _userAccountRepo.GetAccounts(userId);
            return  _mapper.Map<GetUserAccountResponse>(userAccount);
        }

        public async Task CreateAccount(Guid userId, CreateUserAccountRequest userAccountDto)
        {
            userAccountDto.Validate();

            var userTask = _userRepo.GetUser(userId);
            var userAccountTask = _userAccountRepo.GetAccounts(userId);
            await Task.WhenAll(userTask, userAccountTask);

            var user = userTask.Result;
            var userAccount = userAccountTask.Result;
            //TODO CreateUserAccountValidator.Validate(user, userAccount, userAccountDto);

            var targetUserAccount = _mapper.Map<Account>(userAccountDto);
            targetUserAccount.UserId = user.Id;
            await _userAccountRepo.InsertAccount(targetUserAccount);
        }
    }
}