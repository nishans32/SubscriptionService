using System;
using SubscriptionService.Web.Models.Enum;

namespace SubscriptionService.Web.Models.DTO.Query
{
    public class GetAccountResponse
    {
        public Guid UserId { get; set; }
        public AccountType AccountType { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal RepaymentAmount { get; set; }
        public RepaymentFrequency RepaymentFrequency { get; set; }
    }
}
