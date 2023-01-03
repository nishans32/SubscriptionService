using MediatR;
using SubscriptionService.Web.Models.DTO;
using System;

namespace SubscriptionService.Web.Models.DTO.Query
{
    public class GetUserRequest : IRequest<GetUserResponse>
    {
        public Guid UserId { get; set; }
    }
}
