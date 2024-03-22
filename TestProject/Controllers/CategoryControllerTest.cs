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
    public class CategoryControllerTest
    {
        private ICategoryRepository _repository;
        private IMapper _mapper;
        private CategoryController _controller;
        public CategoryControllerTest()
        {
            _repository = A.Fake<ICategoryRepository>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            _mapper = config.CreateMapper();
            _controller = new CategoryController(_repository, _mapper);
        }

        [Fact]
        public async Task PostCategory_ReturnsOkResult()
        {
            // Arrange
            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            var categoryDto = new CategoryAddDTO
            {
                Name = "Food",
                Description = "Expenses related to food items"
            };

            // Act
            A.CallTo(() => _repository.SaveChanges()).Returns(true);
            var result = await _controller.PostCategory(categoryDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostCategory_ReturnsErrorResult()
        {
            // Arrange
            ControllerTestHelper.SetAuthenticatedUser(_controller, "testuser");

            // Act
            var result = await _controller.PostCategory(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult?.Value.Should().Be("Object null");
            badRequestResult?.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetCategoriesByUser_ReturnsOkResult_WhenCategoriesExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingCategories = new List<Category>
    {
        new Category
        {
            CategoryId = 1,
            UserId = userId,
            Name = "Food",
            Description = "Expenses related to food items"
        },
    };

            A.CallTo(() => _repository.GetCategoriesByUser(userId)).Returns(existingCategories);

            // Act
            var result = await _controller.GetCategoriesByUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, _controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Fact]
        public async Task GetCategoriesByUser_ReturnsNotFoundResult_WhenCategoriesDoNotExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var existingCategories = new List<Category>();

            A.CallTo(() => _repository.GetCategoriesByUser(userId)).Returns(existingCategories);

            // Act
            var result = await _controller.GetCategoriesByUser();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Category>>>(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetCategoriesByUser_Exception_WhenCategoriesDoNotExist()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);
            A.CallTo(() => _repository.GetOnlyUserCategories(userId)).Throws<Exception>();

            // Act
            var result = await _controller.GetCategoriesByUser();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task PutCategory_ReturnsOkResult()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var categoryId = 1;
            var categoryDTO = new CategoryAddDTO
            {
                Name = "Food",
                Description = "Expenses related to food items"
            };
            var category = new Category
            {
                CategoryId = categoryId,
                Name = "Food",
                UserId = userId,
                Description = "Expenses related to food items"
            };

            A.CallTo(() => _repository.GetCategoryById(categoryId)).Returns(category);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.PutCategory(categoryId, categoryDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var response = Assert.IsAssignableFrom<Category>(okResult.Value);
            Assert.Equal(category.CategoryId, response.CategoryId);
            Assert.Equal(category.Name, response.Name);
            Assert.Equal(category.Description, response.Description);
        }

        [Fact]
        public async Task PutCategory_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "testuser";
            ControllerTestHelper.SetAuthenticatedUser(_controller, userId);

            var categoryId = 1;
            var categoryDTO = new CategoryAddDTO
            {
                Name = "Food",
                Description = "Expenses related to food items"
            };

            A.CallTo(() => _repository.GetCategoryById(categoryId)).Returns<Category>(null);

            // Act
            var result = await _controller.PutCategory(categoryId, categoryDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Category not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOkResult()
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

            var categoryId = 1;

            var category = new Category
            {
                CategoryId = categoryId,
                UserId = userId,
                Name = "Food",
                Description = "Expenses related to food items"
            };

            A.CallTo(() => _repository.GetCategoryById(categoryId)).Returns(category);
            A.CallTo(() => _repository.SaveChanges()).Returns(true);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Category deleted", okResult.Value);
        }

    }
}
