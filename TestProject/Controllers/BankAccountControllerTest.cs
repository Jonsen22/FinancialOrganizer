using AutoMapper;
using Azure;
using BackEndTest.Controllers;
using BackEndTest.DTOs;
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
        [Fact]
        public async Task PostBankAccount_ReturnsOkResult()
        {
            //Arrange
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var bankAccountController = new BankAccountsController(repository, mapper);

            ControllerTestHelper.SetAuthenticatedUser(bankAccountController, "testuser");

            var bankAccountDto = new BankAccountAddDTO
            {
                Balance = 1500,
                Name = "Nubank"
            };

            var result = await bankAccountController.PostBankAccount(bankAccountDto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);

        }

        [Fact]
        public async Task PostBankAccount_ReturnsErrorResult()
        {
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var bankAccountController = new BankAccountsController(repository, mapper);

            ControllerTestHelper.SetAuthenticatedUser(bankAccountController, "testuser");

            var result = await bankAccountController.PostBankAccount(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.Value.Should().Be("Object null");
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetBankAccountsByUser_ReturnsOkResult_WhenBankAccountExists()
        {
            // Arrange
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var bankAccountController = new BankAccountsController(repository, mapper);

            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(bankAccountController, userId);

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

            A.CallTo(() => repository.GetBankAccountsByUser(userId)).Returns(existingBankAccounts);

            // Act
            var result = await bankAccountController.GetBankAccountsByUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, bankAccountController.User.FindFirstValue(ClaimTypes.NameIdentifier));

        }

        [Fact]
        public async Task GetBankAccountsByUser_ReturnsNotFoundResult_WhenBankAccountDoesNotExist()
        {
            // Arrange
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var bankAccountController = new BankAccountsController(repository, mapper);

            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(bankAccountController, userId);

            var existingBankAccounts = new List<BankAccount>();

            A.CallTo(() => repository.GetBankAccountsByUser(userId)).Returns(existingBankAccounts);

            // Act
            var result = await bankAccountController.GetBankAccountsByUser();

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
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var bankAccountController = new BankAccountsController(repository, mapper);

            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(bankAccountController, userId);
            A.CallTo(() => repository.GetBankAccountsByUser(userId)).Throws<Exception>();

            // Act
            var result = await bankAccountController.GetBankAccountsByUser();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public async Task PutBankAccount_ReturnsOkResult()
        {
            // Arrange
            var repository = A.Fake<IBankAccountRepository>();
            var mapper = A.Fake<IMapper>();
            var controller = new BankAccountsController(repository, mapper);

            var userId = "testuser";
            var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            controller.ControllerContext = new ControllerContext
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

            A.CallTo(() => repository.GetBankAccountById(bankAccountId)).Returns(bankAccount);
            A.CallTo(() => mapper.Map<BankAccount>(A<BankAccountAddDTO>._))
                .Returns(bankAccount);
            A.CallTo(() => repository.SaveChanges()).Returns(true);

            // Act
            var result = await controller.PutBankAccount(bankAccountId, bankAccountDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult); 
            var response = Assert.IsAssignableFrom<BankAccount>(okResult.Value);
            //Assert.Equal(bankAccount.BankAccountId, response.BankAccountId);
            Assert.Equal(bankAccount.Name, response.Name);
            Assert.Equal(bankAccount.Balance, response.Balance);


        }
    }
}
