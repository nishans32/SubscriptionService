using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.Enum;
using static SubscriptionService.Web.Validations.ValidationMessages;
using static SubscriptionService.Web.Validations.Validations;

namespace SubscriptionService.Web.Validators
{
    public static class CreateUserAccountDtoValidation
    {
        public static void Validate(this CreateUserAccountRequest createUserAccountDto)
        {
            if (!IsValidAccountType(createUserAccountDto.AccountType))
                throw new ValidationException(string.Format(INVALID_ACCOUNT_TYPE, createUserAccountDto.AccountType));

            if (createUserAccountDto.LoanAmount > MAXIMUM_LOAN_AMOUNT || createUserAccountDto.LoanAmount < 0)
                throw new ValidationException(string.Format(INVALID_LOAN_AMOUNT, createUserAccountDto.LoanAmount));

            if (!IsValidRepaymentFrequency(createUserAccountDto.RepaymentFrequency))
                throw new ValidationException(string.Format(INVALID_REPYMENT_FREQUENCY, createUserAccountDto.RepaymentFrequency));

            if (createUserAccountDto.RepaymentAmount < 0)
                throw new ValidationException(NEGATIVE_REPAYMENT_AMOUNT);

            if (createUserAccountDto.InterestRate < 0)
                throw new ValidationException(NEGATIVE_INTEREST_RATE);

        }

        private static bool IsValidRepaymentFrequency(RepaymentFrequency repaymentFrequency)
        {
            return Enum.IsDefined(typeof(RepaymentFrequency), repaymentFrequency);
        }

        private static bool IsValidAccountType(AccountType accountType)
        {
            return Enum.IsDefined(typeof(AccountType), accountType);
        }
    }
}
