using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Models.DTO.Commands;
using static SubscriptionService.Web.Validations.ValidationMessages;

namespace SubscriptionService.Web.Validators
{
    public static class CreateUserDtoValidator
    {
        public static void Validate(this CreateUserRequest createUserDto)
        {
            if(String.IsNullOrEmpty(createUserDto.Name))
                throw new ValidationException(USER_NAME_NULL);

            if(!IsValidEmail(createUserDto.Email))
                throw new ValidationException(INVALID_EMAIL);

            if (createUserDto.Salary < 0)
                throw new ValidationException(NEGATIVE_SALARY);

            if(createUserDto.Expenses < 0)
                throw new ValidationException(NEGATIVE_EXPENCES);
        }

        private static bool IsValidEmail(string email)
        {
            return !String.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
              
        }
    }
}
