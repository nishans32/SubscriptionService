using System.Collections;
using System.Collections.Generic;

namespace SubscriptionService.Web.Models.DTO.Query
{
    public class GetUserAccountResponse
    {
        public GetUserResponse User { get; set; }
        public IEnumerable<GetAccountResponse> Accounts { get; set; }
    }
}
