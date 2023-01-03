using MediatR;
using SubscriptionService.Web.Models.DTO;
using System;

namespace SubscriptionService.Web.Models.DTO.Query
{
    public class GetUserAccountRequest : IRequest<GetUserAccountResponse>
    {
        public Guid UserId { get; set; }
    }
}
