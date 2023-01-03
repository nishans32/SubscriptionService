using AutoMapper;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionService.Web.Exceptions;
using SubscriptionService.Web.Mappers;
using SubscriptionService.Web.Models.DAO;
using SubscriptionService.Web.Models.DTO;
using SubscriptionService.Web.Repositories;
using SubscriptionService.Web.Services;
using Xunit;
using static SubscriptionService.Web.Validations.ValidationMessages;

namespace SubscriptionService.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
            _userRepo = Substitute.For<IUserRepository>();
            _userService = new UserService(_userRepo, _mapper);
        }
        [Fact]
        public async Task GetUSer_ShouldReturnUser_WhenUserIdIsValid()
        {
            //Arrange
            var userId = Guid.NewGuid();

            var expectedResult = new User
            {
                Name = "Test user",
                Email = "testemail@gmail.com",
                ExternalUserId = userId,
                Salary = 100.50M,
                Expenses = 50.12M
            };

            _userRepo.GetUser(Arg.Is<Guid>(id => id == userId)).Returns(expectedResult);

            //Act

            var result = await _userService.GetUser(userId);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(expectedResult.Name, result.Name);
            Assert.Equal(expectedResult.Email, result.Email);
            Assert.Equal(expectedResult.ExternalUserId, result.Id);
            Assert.Equal(expectedResult.Salary, result.Salary);
            Assert.Equal(expectedResult.Expenses, result.Expenses);
        }

        [Fact]
        public async Task GetUSer_ShouldReturnNull_WhenUserIdIsValid()
        {
            //Arrange
            var resultUserId = Guid.NewGuid();
            var inputUserId = Guid.NewGuid();
            var expectedResult = new User
            {
                Name = "Test user",
                Email = "testemail@gmail.com",
                ExternalUserId = resultUserId,
                Salary = 100.50M,
                Expenses = 50.12M
            };

            _userRepo.GetUser(Arg.Is<Guid>(id => id == resultUserId)).Returns(expectedResult);

            //Act

            var result = await _userService.GetUser(inputUserId);

            //Assert

            Assert.Null(result);
        }

        [Fact]
        public async void ListUsers_ShouldReturnListOfAllUsers_WhenThereAreUSers()
        {
            //Arrange

            var expectedResult = new List<User>
            {
                new User
                {
                    ExternalUserId = Guid.NewGuid(),
                    Name = "Test user1",
                    Email = "testemail1@gmail.com",
                    Salary = 100.50M,
                    Expenses = 50.12M
                },
                new User
                {
                    ExternalUserId = Guid.NewGuid(),
                    Name = "Test user2",
                    Email = "testemail2@gmail.com",
                    Salary = 100.50M,
                    Expenses = 50.12M
                }
            };

            _userRepo.GetAllUsers().Returns(expectedResult);

            //Act

            var result = await _userService.ListUsers();

            //Assert
            Assert.NotNull(expectedResult);
            Assert.Equal(expectedResult.Count, result.ToList().Count);
        }

        [Fact]
        public async void ListUsers_ShouldNotReturnUsers_WhenThereAreExistingUsers()
        {
            //Arrange

            var expectedResult = new List<User>();

            _userRepo.GetAllUsers().Returns(expectedResult);

            //Act

            var result = await _userService.ListUsers();

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Any());
        }

        [Fact]
        public async void CreateUser_ShouldSaveSuccessfully_WhenInputIsValid()
        {
            var validEmail = "random@email.com";
            var validInput = new CreateUserDto
            {
                Email = validEmail,
                Name = "Random Surname ",
                Salary = 10.60M,
                Expenses = 4.76M
            };

            _userRepo.GetUserByEmail(Arg.Is<string>(email => email == validEmail)).ReturnsNull();

            //Act
            await _userService.CreateUser(validInput);

            //Assert
            await _userRepo.Received().InsertUser(Arg.Is<User>(user =>
                user.Email == validInput.Email &&
                user.Name == validInput.Name &&
                user.Salary == validInput.Salary &&
                user.Expenses == validInput.Expenses
            ));
        }

        [Theory] 
        [MemberData(nameof(TestDataForCreateUserRequestWithNegativeInputs.TestData), MemberType = typeof(TestDataForCreateUserRequestWithNegativeInputs))]
        public async void CreateUser_ShouldRaiseValidationException_WhenInputIsInvalid(string name, string email, decimal salary, decimal expences, string expectedValidationMessage)
        {
            var invalidInput = new CreateUserDto
            {
                Email = email,
                Name = name,
                Salary = salary,
                Expenses = expences
            };

            //Act
            var ex = await Record.ExceptionAsync(() => _userService.CreateUser(invalidInput));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            Assert.Contains(expectedValidationMessage, ex.Message);
            await _userRepo.DidNotReceive().InsertUser(Arg.Is<User>(user => false));
        }

        [Fact]
        public async void CreateUser_ShouldRaiseEmailAddressValidationException_WhenEmailAddressExists()
        {
            var invalidEmail = "random@email.com";
            var invalidInput = new CreateUserDto
            {
                Email = invalidEmail,
                Name = "Random Surname ",
                Salary = 10.60M,
                Expenses = 4.76M
            };

            var userWithEmailAddressInUse = new User
            {
                Email = invalidEmail
            };

            _userRepo.GetUserByEmail(Arg.Is<string>(email => email == invalidEmail)).Returns(userWithEmailAddressInUse);

            //Act
            var ex = await Record.ExceptionAsync(() => _userService.CreateUser(invalidInput));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ValidationException>(ex);
            await _userRepo.DidNotReceive().InsertUser(Arg.Is<User>(user => false));
        }

        public class TestDataForCreateUserRequestWithNegativeInputs
        {
            public static IEnumerable<object[]> TestData
            {
                get 
                {
                    var emptyName = string.Empty;
                    yield return new object[] { emptyName, "test@gmail.com", 12.13M, 13.43M, USER_NAME_NULL };

                    var invalidEmail = "test@g@mail.com";
                    yield return new object[] { "test user", invalidEmail, 12.13M, 13.43M, INVALID_EMAIL };

                    var negativeSalary = -1222.23;
                    yield return new object[] { "test user", "test@gmail.com", negativeSalary, 13.43M, NEGATIVE_SALARY };

                    var negativeExpences = -142.54;
                    yield return new object[] { "test user", "test@gmail.com", 1222.76, negativeExpences, NEGATIVE_EXPENCES };

                }
            }
        }
    }
}
