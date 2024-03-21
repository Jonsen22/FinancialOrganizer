using AutoMapper;
using Azure;
using BackEndTest.Controllers;
using BackEndTest.DTOs;
using BackEndTest.Helpers;
using BackEndTest.Repository.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestProject.Helper;

namespace TestProject.Controllers
{
    public class BankAccountControllerTest
    {
        private IBankAccountRepository _repository;
        private IMapper _mapper;
        private BankAccountsController _controller;
        public BankAccountControllerTest()
        {
            _repository = A.Fake<IBankAccountRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            _mapper = config.CreateMapper();
            _controller = new BankAccountsController(_repository, _mapper);
        }
        [Fact]
        public async Task PostBankAccount_ReturnsOkResult()
        {
            //Arrange
            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            var bankAccountDto = new BankAccountAddDTO
            {
                Balance = 1500,
                Name = "Nubank"
            };

            var result = await _controller.PostBankAccount(bankAccountDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);

        }

        [Fact]
        public async Task PostBankAccount_ReturnsErrorResult()
        {

            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            var result = await _controller.PostBankAccount(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.Value.Should().Be("Object null");
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetBankAccountsByUser_ReturnsOkResult_WhenBankAccountExists()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingBankAccounts = new List<BankAccount>
            {
                new BankAccount
                {
                    BankAccountId = 1,
                    UserId = userId,
                    Name = "Nubank",
                    Balance = 1500
                },
            };

            A.CallTo(() => _repository.GetBankAccountsByUser(userId)).Returns(existingBankAccounts);

            // Act
            var result = await _controller.GetBankAccountsByUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, _controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

        }

        [Fact]
        public async Task GetBankAccountsByUser_ReturnsNotFoundResult_WhenBankAccountDoesNotExist()
        {
            // Arrange

            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingBankAccounts = new List<BankAccount>();

            A.CallTo(() => _repository.GetBankAccountsByUser(userId)).Returns(existingBankAccounts);

            // Act
            var result = await _controller.GetBankAccountsByUser();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<BankAccountDTO>>>(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetBankAccountsByUser_Exception_WhenBankAccountDoesNotExist()
        {
            // Arrange

            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);
            A.CallTo(() => _repository.GetBankAccountsByUser(userId)).Throws<Exception>();

            // Act
            var result = await _controller.GetBankAccountsByUser();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task PutBankAccount_ReturnsOkResult()
        {
            // Arrange

            var userId = "testuser";
            var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            var bankAccountId = 1;
            var bankAccountDTO = new BankAccountAddDTO
            {
                Name = "teste",
                Balance = 300
            };
            var bankAccount = new BankAccount
            {
                BankAccountId = bankAccountId,
                Name = "teste",
                UserId = userId,
                Balance = 5000
            };

            A.CallTo(() => _repository.GetBankAccountById(bankAccountId)).Returns(bankAccount);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.PutBankAccount(bankAccountId, bankAccountDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var response = Assert.IsAssignableFrom<BankAccount>(okResult.Value);
            Assert.Equal(bankAccount.BankAccountId, response.BankAccountId);
            Assert.Equal(bankAccount.Name, response.Name);
            Assert.Equal(bankAccount.Balance, response.Balance);


        }

        [Fact]
        public async Task PutBankAccount_ReturnsNotFound()
        {
            // Arrange

            var userId = "testuser";
            var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            var bankAccountId = 1;
            var bankAccountDTO = new BankAccountAddDTO
            {
                Name = "teste",
                Balance = 300
            };

            var bankAccount = new BankAccount
            {
                BankAccountId = bankAccountId,
                Name = "teste",
                UserId = userId,
                Balance = 5000
            };

            var bankAccountUpdated = new BankAccount
            {
                BankAccountId = bankAccountId,
                Name = "teste",
                UserId = userId,
                Balance = 300
            };

            A.CallTo(() => _repository.GetBankAccountById(bankAccountId)).Returns(bankAccount);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);


            // Act
            var result = await _controller.PutBankAccount(bankAccountId, bankAccountDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var response = Assert.IsAssignableFrom<BankAccount>(okResult.Value);
            //Assert.Equal(bankAccount.BankAccountId, response.BankAccountId);
            Assert.Equal(bankAccount.Name, response.Name);
            Assert.Equal(bankAccount.Balance, response.Balance);
        }

        [Fact]
        public async Task DeleteBankAccount_ReturnsOkResult()
        {
            // Arrange

            var userId = "testuser";
            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId)
    };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            var bankAccountId = 1;

            // Simulate successful deletion

            var bankaccount = new BankAccount
            {
                BankAccountId = bankAccountId,
                UserId = userId,
                Name = "Nubank",
                Balance = 1500
            };

            A.CallTo(() => _repository.GetBankAccountById(bankAccountId)).Returns(bankaccount);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.DeleteBankAccount(bankAccountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Bank Account deleted", okResult.Value);
        }


    }
}
