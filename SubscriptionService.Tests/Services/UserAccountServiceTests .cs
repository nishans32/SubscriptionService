using AutoMapper;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionService.Tests.Models;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Mappers;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Models.DTO;
using SubscriptionService.Web.Models.Enum;
using SubscriptionService.Web.Repositories;
using SubscriptionService.Web.Services;
using Xunit;
using static SubscriptionService.Web.Validations.ValidationMessages;
using static SubscriptionService.Web.Validations.Validations; 

namespace SubscriptionService.Tests.Services
{
    public class UserAccountServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IUserAccountRepository _userAccountRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserAccountService _userAccountService;

        public UserAccountServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
            _userAccountRepo = Substitute.For<IUserAccountRepository>();
            _userRepo = Substitute.For<IUserRepository>();
            _userAccountService = new UserAccountService(_userAccountRepo, _mapper, _userRepo);
        }

        [Fact]
        public async Task GetUserAccounts_ShouldReturnRelatedUserAccounts_WhenUserIdIsValid()
        {
            //Arrange
            var inputUserId = Guid.NewGuid();
            var internalUserID = 1212;
            var expectedResult = new List<Account>
            {
                new Account
                {
                    AccountType = AccountType.CreditCard,
                    UserId = internalUserID,
                    InterestRate = 5.06M,
                    LoanAmount = 750.00M,
                    RepaymentAmount = 32.50M,
                    RepaymentFrequency = RepaymentFrequency.Fortnightly
                },
                new Account
                {
                    AccountType = AccountType.Overdraw,
                    UserId = internalUserID,
                    InterestRate = 4.65M,
                    LoanAmount = 1250.00M,
                    RepaymentAmount = 50.50M,
                    RepaymentFrequency = RepaymentFrequency.Monthly
                },
            };

            _userAccountRepo.GetAccounts(Arg.Is<Guid>(userId => userId == inputUserId)).Returns(expectedResult);

            //Act

            var result = await _userAccountService.GetAccounts(inputUserId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Count, result.ToList().Count);
        }

        [Fact]
        public async Task GetUserAccounts_ShouldReturnNull_WhenUserIdIsValid()
        {
            //Arrange
            var sourceUserId = Guid.NewGuid();
            var internalUserId = 1212;
            var invalidUserID = Guid.NewGuid();
            var expectedResult = new List<Account>
            {
                new Account
                {
                    AccountType = AccountType.CreditCard,
                    UserId = internalUserId,
                    InterestRate = 5.06M,
                    LoanAmount = 750.00M,
                    RepaymentAmount = 32.50M,
                    RepaymentFrequency = RepaymentFrequency.Fortnightly
                },
                new Account
                {
                    AccountType = AccountType.Overdraw,
                    UserId = internalUserId,
                    InterestRate = 4.65M,
                    LoanAmount = 1250.00M,
                    RepaymentAmount = 50.50M,
                    RepaymentFrequency = RepaymentFrequency.Monthly
                },
            };

            _userAccountRepo.GetAccounts(Arg.Is<Guid>(userId => userId == sourceUserId)).Returns(expectedResult);

            //Act
            var result = await _userAccountService.GetAccounts(invalidUserID);

            //Assert
            Assert.True(!result.Any());
        }

        [Fact]
        public async void SaveUserAccounts_ShouldSaveSuccessfully_WhenInputIsValid()
        {
            //Arrange
            var validUserId = Guid.NewGuid();
            var requestedAccountType = AccountType.Overdraw;
            var validUserAccountDto = new CreateUserAccountDto
            {
                UserId = validUserId,
                AccountType = requestedAccountType,
                RepaymentFrequency = RepaymentFrequency.Fortnightly,
                InterestRate = 1.25M,
                LoanAmount = 900.00M,
                RepaymentAmount = 33.33M
            };

            var userAccountWithoutRequestedAccountType = new List<Account>
            {
                new Account
                {
                    AccountType = AccountType.CreditCard
                }
            };

            var user = new User() { ExternalUserId = validUserId, Salary = 10000.50M, Expenses = 1000.73M};
            _userRepo.GetUser(Arg.Is<Guid>(userId => userId == validUserId)).Returns(user);
            _userAccountRepo.GetAccounts(Arg.Is<Guid>(userId => userId == validUserId)).Returns(userAccountWithoutRequestedAccountType);

            //Act
            await _userAccountService.CreateAccount(validUserAccountDto.UserId, validUserAccountDto);

            //Assert
            await _userAccountRepo.Received().InsertAccount(Arg.Is<Account>(user =>
                user.AccountType == validUserAccountDto.AccountType &&
                user.InterestRate == validUserAccountDto.InterestRate &&
                user.LoanAmount == validUserAccountDto.LoanAmount &&
                user.RepaymentAmount == validUserAccountDto.RepaymentAmount &&
                user.RepaymentFrequency == validUserAccountDto.RepaymentFrequency));
        }

        [Theory]
        [ClassData(typeof(TestDataProviderForUserAccountValidation))]
        public async void SaveUserAccounts_ShouldRaiseValidationException_WhenInputContainsAnInvalidValues(UserAccountTestData testData)
        {
            //Arrange
            var accountDto = new CreateUserAccountDto
            {
                UserId = Guid.NewGuid(),
                AccountType = testData.AccountType,
                RepaymentFrequency = testData.RepaymentFrequency,
                InterestRate = testData.InterestRate,
                RepaymentAmount = testData.RepaymentAmount,
                LoanAmount = testData.LoanAmount
            };

            //Act
            var ex = await Record.ExceptionAsync(async () => await _userAccountService.CreateAccount(accountDto.UserId, accountDto));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(testData.ExpectedValidationMessage, ex.Message);
            await _userAccountRepo.DidNotReceive().InsertAccount(Arg.Is<Account>(user => false));
        }

        [Fact]
        public async void SaveUserAccounts_ShouldRaiseValidationException_WhenInputContainsAnInvalidRepaymentFrequency()
        {
            //Arrange
            var accountDtoWithInvalidRepaymentFrequency = new CreateUserAccountDto
            {
                AccountType = AccountType.Overdraw,
                RepaymentFrequency = (RepaymentFrequency)1099
            };
            var expectedValidationMessage = string.Format(INVALID_REPYMENT_FREQUENCY, accountDtoWithInvalidRepaymentFrequency.RepaymentFrequency);

            //Act
            var ex = await Record.ExceptionAsync(async () => await _userAccountService.CreateAccount(accountDtoWithInvalidRepaymentFrequency.UserId, accountDtoWithInvalidRepaymentFrequency));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(expectedValidationMessage, ex.Message);
            await _userAccountRepo.DidNotReceive().InsertAccount(Arg.Is<Account>(user => false));
        }

        [Fact]
        public async void SaveUserAccounts_ShouldRaiseValidationException_WhenUserAddsADuplicateAccount()
        {
            //Arrange
            const AccountType accountTypeThatUserAlreadyHas = AccountType.Overdraw;
            var offendingDto = new CreateUserAccountDto
            {
                UserId = Guid.NewGuid(),
                AccountType = accountTypeThatUserAlreadyHas,
                RepaymentFrequency = RepaymentFrequency.Fortnightly
            };

            var userAccountsMockContainingTheRequestedAccountType = new List<Account>
            {
                new Account
                {
                    AccountType = accountTypeThatUserAlreadyHas
                }
            };

            var expectedValidationMessage = string.Format(USER_ACCOUNT_EXISTS, offendingDto.UserId, offendingDto.AccountType);

            _userAccountRepo.GetAccounts(Arg.Is<Guid>(userId => userId == offendingDto.UserId)).Returns(userAccountsMockContainingTheRequestedAccountType);

            //Act
            var ex = await Record.ExceptionAsync(async () => await _userAccountService.CreateAccount(offendingDto.UserId, offendingDto));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(expectedValidationMessage, ex.Message);
            await _userAccountRepo.DidNotReceive().InsertAccount(Arg.Is<Account>(user => false));
        }

        [Fact]
        public async void SaveUserAccounts_ShouldRaiseValidationException_WhenUserIdisInvalid()
        {
            //Arrange
            var validUserId = Guid.NewGuid();
            var invalidUserId = Guid.NewGuid();

            var offendingDto = new CreateUserAccountDto
            {
                UserId = invalidUserId,
                AccountType = AccountType.Overdraw,
                RepaymentFrequency = RepaymentFrequency.Fortnightly
            };

            var user = new User() { ExternalUserId = validUserId };

            var expectedValidationMessage = string.Format(INVALID_USER, offendingDto.UserId);

            _userRepo.GetUser(Arg.Is<Guid>(userId => userId == validUserId)).Returns(user);

            //Act
            var ex = await Record.ExceptionAsync(async () => await _userAccountService.CreateAccount(offendingDto.UserId, offendingDto));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(expectedValidationMessage, ex.Message);
            await _userAccountRepo.DidNotReceive().InsertAccount(Arg.Is<Account>(user => false));
        }


        [Fact]
        public async void SaveUserAccounts_ShouldRaiseValidationException_WhenUsersEligibilityIsNotMet()
        {
            //Arrange
            var usersSalary = 1000;
            var ineligibleExpences = usersSalary - EXPECTED_SALARY_BUFFER + 10;
            var userId = Guid.NewGuid();

            var userAccountDto = new CreateUserAccountDto
            {
                UserId = userId,
                AccountType = AccountType.Overdraw,
                RepaymentFrequency = RepaymentFrequency.Fortnightly
            };

            var ineligibleUser = new User() 
            { 
                ExternalUserId = userId,
                Salary = usersSalary,
                Expenses = ineligibleExpences
            };

            var expectedValidationMessage = string.Format(UNSATISFACTORY_LOAN_AFORDABILITY, userAccountDto.UserId);

            _userRepo.GetUser(Arg.Is<Guid>(id => id == userId)).Returns(ineligibleUser);

            //Act
            var ex = await Record.ExceptionAsync(async () => await _userAccountService.CreateAccount(userAccountDto.UserId, userAccountDto));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(expectedValidationMessage, ex.Message);
            await _userAccountRepo.DidNotReceive().InsertAccount(Arg.Is<Account>(user => false));
        }

        public class TestDataProviderForUserAccountValidation: IEnumerable<object[]>
        {
                public IEnumerator<object[]> GetEnumerator()
                {
                    var invalidAccountType = (AccountType)1000;
                    var testDataWithInvalidAccountType = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = invalidAccountType,
                            RepaymentFrequency = RepaymentFrequency.Annually,
                            LoanAmount = MAXIMUM_LOAN_AMOUNT -1,
                            InterestRate = 1.0M,
                            RepaymentAmount = 100.12M,
                            ExpectedValidationMessage = string.Format(INVALID_ACCOUNT_TYPE, invalidAccountType)
                        }                        
                    };
                    yield return testDataWithInvalidAccountType;

                    var invalidRepaymentFrequency = (RepaymentFrequency)1000;
                    var testDataWithInvalidRepaymentFrequency = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = AccountType.CreditCard,
                            RepaymentFrequency = invalidRepaymentFrequency,
                            LoanAmount = MAXIMUM_LOAN_AMOUNT -1,
                            InterestRate = 1.0M,
                            RepaymentAmount = 100.12M,
                            ExpectedValidationMessage = string.Format(INVALID_REPYMENT_FREQUENCY, invalidRepaymentFrequency)
                        }
                    };
                    yield return testDataWithInvalidRepaymentFrequency;

                    var invalidLoanAmount = MAXIMUM_LOAN_AMOUNT + 1;
                    var testDataWithInvalidLoanAmount = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = AccountType.CreditCard,
                            RepaymentFrequency = RepaymentFrequency.Annually,
                            LoanAmount = invalidLoanAmount,
                            InterestRate = 1.0M,
                            RepaymentAmount = 100.12M,
                            ExpectedValidationMessage = string.Format(INVALID_LOAN_AMOUNT, invalidLoanAmount)
                        }
                    };
                    yield return testDataWithInvalidLoanAmount;

                    var negativeLoanAmount = -120.00M;
                    var testDataWithNegativeLoanAmount = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = AccountType.CreditCard,
                            RepaymentFrequency = RepaymentFrequency.Annually,
                            LoanAmount = negativeLoanAmount,
                            InterestRate = 1.0M,
                            RepaymentAmount = 100.12M,
                            ExpectedValidationMessage = string.Format(INVALID_LOAN_AMOUNT, negativeLoanAmount)
                        }
                    };
                    yield return testDataWithNegativeLoanAmount;

                    var negativeInterestRate = -1.0M;
                    var testDataWithNegativeInterestRate = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = AccountType.CreditCard,
                            RepaymentFrequency = RepaymentFrequency.Annually,
                            LoanAmount = 120.00M,
                            InterestRate = negativeInterestRate,
                            RepaymentAmount = 100.12M,
                            ExpectedValidationMessage = string.Format(NEGATIVE_INTEREST_RATE, negativeInterestRate)
                        }
                    };
                    yield return testDataWithNegativeInterestRate;

                    var negativeRepaymentAmount = -40.32M;
                    var testDataWithNegativeRepaymentAmount = new object[]
                    {
                        new UserAccountTestData
                        {
                            AccountType = AccountType.CreditCard,
                            RepaymentFrequency = RepaymentFrequency.Annually,
                            LoanAmount = 120.00M,
                            InterestRate = 1.0M,
                            RepaymentAmount = negativeRepaymentAmount,
                            ExpectedValidationMessage = string.Format(NEGATIVE_REPAYMENT_AMOUNT, negativeRepaymentAmount)
                        }
                    };
                    yield return testDataWithNegativeRepaymentAmount;
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
