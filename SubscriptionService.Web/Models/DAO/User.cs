using System;

namespace SubscriptionService.Web.Models.DAO
{
    public class User
    {
        public int Id { get; set; }
        public Guid ExternalUserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public decimal Expenses { get; set; }
        public DateTime DateCreated { get; set; }
    }
}