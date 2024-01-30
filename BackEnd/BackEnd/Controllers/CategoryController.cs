using AutoMapper;
using BackEndTest.DTOs;
using BackEndTest.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer.Models;
using Serilog;
using System.Security.Claims;

namespace BackEndTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _repository.GetCategoriesByUser(currentUserId);
            var categoriesReturn = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return categoriesReturn.Any() ? Ok(categoriesReturn) : BadRequest("Error");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpGet("GetOnlyUserCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetUserCategories()
        {
            try
            {

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
                Unauthorized("Not authorized");

            var categories = await _repository.GetOnlyUserCategories(currentUserId);
            var categoriesReturn = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return categoriesReturn.Any() ? Ok(categoriesReturn) : BadRequest("Error");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(CategoryAddDTO categoryDTO)
        {
            try
            {

            if (categoryDTO == null) return BadRequest("No categories found");
            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categoryAdd = _mapper.Map<Category>(categoryDTO);
            categoryAdd.UserId = currentUserId;

            _repository.Add(categoryAdd);

            var response = _mapper.Map<CategoryDTO>(categoryAdd);

            return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutCategory(int categoryId, CategoryAddDTO categoryDTO)
        {
            try
            {
                var category = await _repository.GetCategoryById(categoryId);
                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (category.UserId != currentUserId)
                    Unauthorized("Not authorized");

                var categoryUpdate = _mapper.Map(categoryDTO, category);
                _repository.Update(categoryUpdate);
                var response = _mapper.Map<CategoryDTO>(categoryUpdate);
                return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {

                var categoryDelete = await _repository.GetCategoryById(categoryId);

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (categoryDelete.UserId != currentUserId)
                    Unauthorized("Not authorized");

                _repository.Delete(categoryDelete);

                return await _repository.SaveChanges() ? Ok("Bank Account deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }

        }
    }
}
