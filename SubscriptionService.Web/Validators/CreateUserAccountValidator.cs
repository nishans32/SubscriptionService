using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.Enum;
using static SubscriptionService.Web.Validations.ValidationMessages;
using static SubscriptionService.Web.Validations.Validations;
namespace SubscriptionService.Web.Validators
{
    public static class CreateUserAccountValidator
    {
        public static void Validate(User user, IEnumerable<Account> userAccounts, CreateUserAccountRequest createUserAccountDto)
        {
            userAccounts.ToList().ForEach(userAcc =>
            {
                if (userAcc.AccountType == createUserAccountDto.AccountType)
                    throw new ValidationException(
                        string.Format(USER_ACCOUNT_EXISTS, createUserAccountDto.UserId, createUserAccountDto.AccountType),
                        ValidationCriticality.Warning);
            });

            if (user == null)
                throw new ValidationException(string.Format(INVALID_USER, createUserAccountDto.UserId));

            if (UserCannotAffordLoan(user))
                throw new ValidationException(string.Format(UNSATISFACTORY_LOAN_AFORDABILITY, createUserAccountDto.UserId), ValidationCriticality.Warning);
        }

        private static bool UserCannotAffordLoan(User user)
        {
            return user.Salary - user.Expenses > EXPECTED_SALARY_BUFFER;
        }
    }
}
