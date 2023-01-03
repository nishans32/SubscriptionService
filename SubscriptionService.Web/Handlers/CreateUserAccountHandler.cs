using MediatR;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SubscriptionService.Web.Handlers
{
    public class CreateUserAccountHandler: IRequestHandler<CreateUserAccountRequest>
    {
        private readonly IUserAccountService _userAccountService;

        public CreateUserAccountHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<Unit> Handle(CreateUserAccountRequest createUserAccountRequest, CancellationToken token)
        {
            await _userAccountService.CreateAccount(createUserAccountRequest.UserId, createUserAccountRequest);
            return Unit.Value;
        }

    }
}
