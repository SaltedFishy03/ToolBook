using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Server.DTOs.ToolCategory;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolCategoryController(IToolCategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ToolCategoryResponse>>> GetAllCategories()
        {
            return Ok(await categoryService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolCategoryResponse>> GetCategoryById(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Kategorien med dette id blev ikke fundet");
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolCategoryResponse>> CreateCategory(CreateToolCategoryRequest category)
        {
            var result = await categoryService.CreateAsync(category);

            if (result == null)
            {
                return Conflict("Kategorien findes allerede");
            }

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolCategoryResponse>> UpdateCategory(int id, UpdateToolCategoryRequest category)
        {
            var updatedCategory = await categoryService.UpdateAsync(id, category);
            if (updatedCategory == null)
            {
                return NotFound("Kategorien med dette id blev ikke fundet");
            }

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await categoryService.DeleteAsync(id);

            if (result == null)
            {
                return NotFound("Kategorien med dette id blev ikke fundet");
            }

            if (result == false)
            {
                return Conflict("Kategorien kan ikke slettes, fordi der er værktøjstyper tilknyttet den");
            }

            return NoContent();
        }
    }
}