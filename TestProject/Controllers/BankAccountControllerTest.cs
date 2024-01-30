using AutoMapper;
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
            badRequestResult?.Value.Should().Be("unregistered bank accounts");
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetBankAccount_ReturnsOkResult_WhenBankAccountExists()
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
                // Add more BankAccount instances as needed
            };

            A.CallTo(() => repository.GetBankAccountsByUser(userId)).Returns(existingBankAccounts);

            // Act
            var result = await bankAccountController.GetBankAccountsByUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, bankAccountController.User.FindFirstValue(ClaimTypes.NameIdentifier));

        }

        //[Fact]
        //public async Task GetBankAccount_ReturnsNotFoundResult_WhenBankAccountDoesNotExist()
        //{
        //    // Arrange
        //    var repository = A.Fake<IBankAccountRepository>();
        //    var mapper = A.Fake<IMapper>();
        //    var bankAccountController = new BankAccountsController(repository, mapper);

        //    A.CallTo(() => repository.GetByIdAsync(2)).Returns(null); // Simulate a non-existing bank account

        //    // Act
        //    var result = await bankAccountController.GetBankAccount(2);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}
    }
}
