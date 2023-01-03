using MediatR;
using SubscriptionService.Web.Models.DTO;
using System.Threading;
using System.Threading.Tasks;
using SubscriptionService.Web.Services;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Handlers
{
    public class GetUserHandler : IRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly IUserService _userService;

        public GetUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            return await _userService.GetUser(request.UserId);
        }
    }
}
