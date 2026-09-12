using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToolBook.Server.DTOs.Tool;
using ToolBook.Server.Enums;
using ToolBook.Server.Services.Interfaces;

namespace ToolBook.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToolController(IToolService toolService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ToolResponse>>> GetAllTools()
        {
            return Ok(await toolService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToolResponse>> GetToolById(int id)
        {
            var tool = await toolService.GetByIdAsync(id);
            if (tool == null)
            {
                return NotFound("værktøjet med dette id blev ikke fundet");
            }

            return Ok(tool);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolResponse>> CreateTool(CreateToolRequest tool)
        {
            var result = await toolService.CreateAsync(tool);

            if (result == null)
            {
                return BadRequest("Værktøjet kunne ikke oprettes");
            }

            return CreatedAtAction(nameof(GetToolById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ToolResponse>> UpdateTool(int id, UpdateToolRequest tool)
        {
            var updatedTool = await toolService.UpdateAsync(id, tool);

            if (updatedTool == null)
            {
                return Conflict("kunne ikke opdatere Værktøjet");
            }

            return Ok(updatedTool);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTool(int id)
        {
            var result = await toolService.DeleteAsync(id);

            if (result == null)
            {
                return NotFound("Værktøjet med dette id blev ikke fundet");
            }

            if (result == false)
            {
                return Conflict("Værktøjet kan ikke slettes, fordi der er bookinger tilknyttet det");
            }

            return NoContent();
        }
    }
}