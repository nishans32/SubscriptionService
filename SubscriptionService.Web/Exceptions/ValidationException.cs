using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionService.Web.Exceptions
{
    public class ValidationException: Exception
    {
        public ValidationCriticality Criticality { get; set; }
        public ValidationException(string message, ValidationCriticality criticality = ValidationCriticality.Info) : base(message) 
        {
            this.Criticality = criticality;
        }
    }
    public enum ValidationCriticality
    {
        Info, 
        Warning,
        Critical
    }

}
