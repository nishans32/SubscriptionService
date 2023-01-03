using System.Collections.Generic;

namespace SubscriptionService.Web.Models.DAO
{
    public class UserAccount
    {
        public User User { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
