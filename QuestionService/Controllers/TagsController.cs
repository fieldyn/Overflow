using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Data;
using QuestionService.Models;
using Microsoft.EntityFrameworkCore;

namespace QuestionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagsController(QuestionDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            var tags = await db.Tags.OrderBy(x=>x.Name).ToListAsync();
            return Ok(tags);
        }
    }
}
