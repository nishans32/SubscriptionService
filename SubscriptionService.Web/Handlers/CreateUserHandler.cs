using MediatR;
using SubscriptionService.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SubscriptionService.Web.Models.DTO.Commands;

namespace SubscriptionService.Web.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, Unit>
    {
        private readonly IUserService _userService;

        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            await _userService.CreateUser(request);
            return Unit.Value;
        }
    }
}
