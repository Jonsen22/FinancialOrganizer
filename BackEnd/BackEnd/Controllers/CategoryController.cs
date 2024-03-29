﻿using AutoMapper;
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
        const string errorMessage = "Error: {ErrorMessage}";
        const string UnexpectedError = "Unexpected Error";

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

                if (currentUserId == null)
                    return NotFound("User not Found");

                var categories = await _repository.GetCategoriesByUser(currentUserId);
                var categoriesReturn = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

                return categoriesReturn.Any() ? Ok(categoriesReturn) : BadRequest("Error");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpGet("GetOnlyUserCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByUser()
        {
            try
            {

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == null)
                    return NotFound("User not Found");


                var categories = await _repository.GetOnlyUserCategories(currentUserId);




                return categories.Any() ? Ok(categories) : NotFound("Error");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(CategoryAddDTO categoryDTO)
        {
            try
            {

                if (categoryDTO == null) return BadRequest("Object null");
                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var categoryAdd = _mapper.Map<Category>(categoryDTO);
                categoryAdd.UserId = currentUserId;

                _repository.Add(categoryAdd);

                var response = _mapper.Map<CategoryDTO>(categoryAdd);

                return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutCategory(int categoryId, CategoryAddDTO categoryDTO)
        {
            try
            {
                var category = await _repository.GetCategoryById(categoryId);

                if (category == null) return NotFound("Category not found");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (category.UserId != currentUserId)
                    return Unauthorized("Not authorized");

                var categoryUpdate = _mapper.Map(categoryDTO, category);
                _repository.Update(categoryUpdate);

                return await _repository.SaveChanges() ? Ok(categoryUpdate) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
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

                return await _repository.SaveChanges() ? Ok("Category deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }

        }
    }
}
