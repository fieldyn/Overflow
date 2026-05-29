using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;

namespace QuestionService.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(QuestionDbContext db): ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto dto)
    {
        var validTags = await db.Tags.Where(x=> dto.Tags.Contains(x.Slug)).ToListAsync();
        
        var missing = dto.Tags.Except(validTags.Select(x=> x.Slug).ToList()).ToList();
        
        if (missing.Count() != 0)
            return BadRequest($"Invalid tags: {string.Join(", ", missing)}");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");
        
        if(userId is null || name is null)
            return BadRequest("User information is missing from the token.");

        var question = new Question
        {
            Title = dto.Title,
            Content = dto.Content,
            AskerId = userId,
            AskerDisplayName = name,
            TagSlugs = dto.Tags,
        };

        db.Questions.Add(question);
        await db.SaveChangesAsync();
        return Created($"/question/{question.Id}", question);
    }

    [HttpGet]
    public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
    {
        IQueryable<Question> query = db.Questions;

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(q => q.TagSlugs.Contains(tag));
        }

        var questions = await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
        return Ok(questions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(string id)
    {
        var question = await db.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        await db.Questions.Where(q => q.Id == id)
            .ExecuteUpdateAsync(q => q.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));

        return Ok(question);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(string id)
    {
        var question = await db.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (question.AskerId != user)
        {
            return Forbid();
        }


        db.Questions.Remove(question);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(string id, CreateQuestionDto dto)
    {
        var question = await db.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (question.AskerId != user)
        {
            return Forbid();
        }

        var validTags = await db.Tags.Where(x=> dto.Tags.Contains(x.Slug)).ToListAsync();
        
        var missing = dto.Tags.Except(validTags.Select(x=> x.Slug).ToList()).ToList();
        
        if (missing.Count() != 0)
            return BadRequest($"Invalid tags: {string.Join(", ", missing)}");


        question.Title = dto.Title;
        question.Content = dto.Content;
        question.TagSlugs = dto.Tags;
        question.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return NoContent();
    }
}