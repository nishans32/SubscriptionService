using System;
using MediatR;
using SubscriptionService.Web.Models.Enum;

namespace SubscriptionService.Web.Models.DTO.Commands
{
    public class CreateUserAccountRequest: IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public AccountType AccountType { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal RepaymentAmount { get; set; }
        public RepaymentFrequency RepaymentFrequency { get; set; }
    }
}