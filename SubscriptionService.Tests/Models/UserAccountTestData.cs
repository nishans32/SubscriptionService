using System;
using System.Collections.Generic;
using System.Text;
using SubscriptionService.Web.Models.Enum;

namespace SubscriptionService.Tests.Models
{
    public class UserAccountTestData
    {
        public AccountType AccountType { get; set; }
        public RepaymentFrequency RepaymentFrequency { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal RepaymentAmount { get; set; }
        public decimal InterestRate { get; set; }
        public string ExpectedValidationMessage { get; set; }
    }
}
