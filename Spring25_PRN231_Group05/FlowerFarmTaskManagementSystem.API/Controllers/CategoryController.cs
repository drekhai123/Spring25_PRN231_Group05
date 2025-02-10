using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryRepo;

        public CategoryController(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/Category
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryRepo.GetAllCategories();
            return Ok(categories);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryRepo.GetCategoryById(id);
            if (category == null) return NotFound("Category not found.");
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdCategory = await _categoryRepo.CreateCategory(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedCategory = await _categoryRepo.UpdateCategory(id, category);
            if (updatedCategory == null) return NotFound("Category not found.");
            return Ok(updatedCategory);
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var deletedCategory = await _categoryRepo.DeleteCategory(id);
            if (deletedCategory == null) return NotFound("Category not found.");
            return Ok(deletedCategory);
        }
    }
}
