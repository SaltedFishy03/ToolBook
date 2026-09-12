using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Server.DTOs.ToolCategory;
using ToolBook.Server.DTOs.ToolType;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolTypeController(IToolTypeService typeService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ToolTypeResponse>>> GetAllToolTypes()
        {
            return Ok(await typeService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolTypeResponse>> GetToolTypeById(int id)
        {
            var toolType = await typeService.GetByIdAsync(id);
            if (toolType == null)
            {
                return NotFound("værktøjs typen med dette id blev ikke fundet");
            }

            return Ok(toolType);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolTypeResponse>> CreateToolType(CreateToolTypeRequest toolType)
        {
            var result = await typeService.CreateAsync(toolType);

            if (result == null)
            {
                return Conflict("Kunne ikke oprette værktøjs typen");
            }

            return CreatedAtAction(nameof(GetToolTypeById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolTypeResponse>> UpdateToolType(int id, UpdateToolTypeRequest toolType)
        {
            var updatedToolType = await typeService.UpdateAsync(id, toolType);

            if (updatedToolType == null)
            {
                return Conflict("Kunne ikke opdatere værktøjs typen");
            }

            return Ok(updatedToolType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteToolType(int id)
        {
            var result = await typeService.DeleteAsync(id);

            if (result == null)
            {
                return NotFound("Værktøjstypen med dette id blev ikke fundet");
            }

            if (result == false)
            {
                return Conflict("Værktøjstypen kan ikke slettes, fordi der er værktøjer tilknyttet den");
            }

            return NoContent();
        }
    }
}
