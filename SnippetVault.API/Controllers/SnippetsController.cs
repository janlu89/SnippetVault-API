using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetVault.Application.DTOs.Snippets;
using SnippetVault.Application.Interfaces;

namespace SnippetVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnippetService _snippetService;

        public SnippetsController(ISnippetService snippetService)
        {
            _snippetService = snippetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSnippets([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? language = null, [FromQuery] string? tag = null)
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            Guid? userId = claim != null ? Guid.Parse(claim.Value) : null;
            var result = await _snippetService.GetSnippets(page, pageSize, search, language, tag, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnippetById(Guid id)
        {
            var result = await _snippetService.GetSnippetById(id);
            return Ok(result);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSnippet([FromBody] CreateSnippetRequest request)
        {
            string userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var userId = Guid.Parse(userIdString);

            var result = await _snippetService.CreateSnippet(request, userId);
            return CreatedAtAction(nameof(GetSnippetById), new { id = result.Id }, result);

        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSnippet(Guid id, [FromBody] UpdateSnippetRequest request)
        {
            string userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var userId = Guid.Parse(userIdString);
                        
            var result = await _snippetService.UpdateSnippet(id, request, userId);
            return Ok(result);
            
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSnippet(Guid id)
        {
            string userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var userId = Guid.Parse(userIdString);

            await _snippetService.DeleteSnippet(id, userId);
            return NoContent();            
        }

        [HttpDelete("{id}/hard")]
        [Authorize]
        public async Task<IActionResult> HardDeleteSnippet(Guid id)
        {
            await _snippetService.HardDeleteSnippet(id);
            return NoContent();           
        }
    }
}
