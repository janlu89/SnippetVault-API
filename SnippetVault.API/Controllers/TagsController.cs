using Microsoft.AspNetCore.Mvc;
using SnippetVault.Application.Interfaces;

namespace SnippetVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagsController(ITagService tagService )
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]string? search = null) 
        {
            var tags = _tagService.GetAllTags(search);
            return Ok(tags);
        }
    }
}
