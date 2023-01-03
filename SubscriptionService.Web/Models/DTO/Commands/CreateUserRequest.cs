using MediatR;

namespace SubscriptionService.Web.Models.DTO.Commands
{
    public class CreateUserRequest : IRequest<Unit>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public decimal Expenses { get; set; }
    }
}