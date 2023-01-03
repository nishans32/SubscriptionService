using MediatR;
using SubscriptionService.Web.Models.DTO.Query;
using SubscriptionService.Web.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SubscriptionService.Web.Handlers
{
    public class GetUserAccountHandler : IRequestHandler<GetUserAccountRequest, GetUserAccountResponse>
    {
        private readonly IUserAccountService _userAccountService;

        public GetUserAccountHandler(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }
        public async Task<GetUserAccountResponse> Handle(GetUserAccountRequest request, CancellationToken cancellationToken)
        {
            return await _userAccountService.GetAccounts(request.UserId);
        }
    }
}
