using AutoMapper;
using BackEndTest.Controllers;
using BackEndTest.DTOs;
using BackEndTest.Helpers;
using BackEndTest.Repository.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public class TransactionControllerTest
    {

        private ITransactionRepository _repository;
        private IMapper _mapper;
        private TransactionController _controller;
        public TransactionControllerTest()
        {
            _repository = A.Fake<ITransactionRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            _mapper = config.CreateMapper();
            _controller = new TransactionController(_repository, _mapper);
        }

        [Fact]
        public async Task PostTransaction_ReturnsOkResult()
        {
            // Arrange
            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            var transactionDto = new TransactionAddDTO
            {
                Name = "Transaction 1",
                Value = 100,
                Date = DateTime.Now,
                Recurring = 'N',
                Type = "income"
            };

            // Act
            A.CallTo(() => _repository.SaveChanges()).Returns(true);
            var result = await _controller.PostTransaction(transactionDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostTransaction_ReturnsErrorResult()
        {
            // Arrange
            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            // Act
            var result = await _controller.PostTransaction(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult?.Value.Should().Be("object null");
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetTransactionsByUser_ReturnsOkResult_WhenTransactionsExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingTransactions = new List<Transaction>
    {
        new Transaction
        {
            TransactionId = 1,
            UserId = userId,
            Name = "Transaction 1",
            Value = 100,
            Date = DateTime.Now,
            Recurring = 'N',
            Type = "income"
        },
    };

            A.CallTo(() => _repository.GetTransactionsByUser(userId)).Returns(existingTransactions);

            // Act
            var result = await _controller.GetTransactionsByUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, _controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Fact]
        public async Task GetTransactionsByUser_ReturnsNotFoundResult_WhenTransactionsDoNotExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingTransactions = new List<Transaction>();

            A.CallTo(() => _repository.GetTransactionsByUser(userId)).Returns(existingTransactions);

            // Act
            var result = await _controller.GetTransactionsByUser();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Transaction>>>(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetTransactionsByUser_Exception_WhenTransactionsDoNotExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);
            A.CallTo(() => _repository.GetTransactionsByUser(userId)).Throws<Exception>();

            // Act
            var result = await _controller.GetTransactionsByUser();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task PutTransaction_ReturnsOkResult()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var transactionId = 1;
            var transactionDTO = new TransactionAddDTO
            {
                Name = "Transaction 1",
                Value = 100,
                Date = DateTime.Now,
                Recurring = 'N',
                Type = "income"
            };
            var transaction = new Transaction
            {
                TransactionId = transactionId,
                Name = "Transaction 1",
                UserId = userId,
                Value = 100,
                Date = DateTime.Now,
                Recurring = 'N',
                Type = "income"
            };

            A.CallTo(() => _repository.GetTransactionById(transactionId)).Returns(transaction);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.PutTransaction(transactionId, transactionDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var response = Assert.IsAssignableFrom<Transaction>(okResult.Value);
            Assert.Equal(transaction.TransactionId, response.TransactionId);
            Assert.Equal(transaction.Name, response.Name);
            Assert.Equal(transaction.Value, response.Value);
            Assert.Equal(transaction.Date, response.Date);
            Assert.Equal(transaction.Recurring, response.Recurring);
            Assert.Equal(transaction.Type, response.Type);
        }

        [Fact]
        public async Task PutTransaction_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var transactionId = 1;
            var transactionDTO = new TransactionAddDTO
            {
                Name = "Transaction 1",
                Value = 100,
                Date = DateTime.Now,
                Recurring = 'N',
                Type = "income"
            };

            A.CallTo(() => _repository.GetTransactionById(transactionId)).Returns<Transaction>(null);

            // Act
            var result = await _controller.PutTransaction(transactionId, transactionDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Transaction not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteTransaction_ReturnsOkResult()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);
            var transactionId = 1;

            var transaction = new Transaction
            {
                TransactionId = transactionId,
                Name = "Transaction 1",
                UserId = userId,
                Value = 100,
                Date = DateTime.Now,
                Recurring = 'N',
                Type = "income"
            };

            A.CallTo(() => _repository.GetTransactionById(transactionId)).Returns<Transaction>(transaction);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transaction Deleted", okResult.Value);
        }
    }
}

