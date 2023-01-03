namespace SubscriptionService.Web.Validations
{
    public static class ValidationMessages
    {
        public const string USER_NAME_NULL = "User name cannot be empty";
        public const string INVALID_EMAIL = "Invalid email address";
        public const string USER_ACCOUNT_EXISTS = "User: {0} already has an account type:{1}";
        public const string INVALID_USER = "Invalid user: {0}";
        public const string NEGATIVE_SALARY = "Salary must be a positive value";
        public const string NEGATIVE_EXPENCES = "Expences must be a positive value";

        public const string INVALID_ACCOUNT_TYPE = "Invalid account type: {0}";
        public const string INVALID_LOAN_AMOUNT = "Invalid loan amount: {0}";
        public const string UNSATISFACTORY_LOAN_AFORDABILITY = "Not satisfying loan affordability criteria";
        public const string INVALID_REPYMENT_FREQUENCY = "Invalid repayment frequency: {0}";
        public const string NEGATIVE_REPAYMENT_AMOUNT = "Repayment amount must be a positive value";
        public const string NEGATIVE_INTEREST_RATE = "Interest rate must be a positive value";
    }

    public static class Validations
    {
        public const decimal EXPECTED_SALARY_BUFFER = 1000;
        public const decimal MAXIMUM_LOAN_AMOUNT = 1000;
    }
}
