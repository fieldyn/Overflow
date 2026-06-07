using System.Security.Claims;
using FastExpressionCompiler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;
using QuestionService.Services;
using Wolverine;

namespace QuestionService.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(QuestionDbContext db, IMessageBus bus, TagService tagService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto dto)
    {
        if (!await tagService.AreTagsValidAsync(dto.Tags))
        {
            return BadRequest("One or more tags are invalid.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");

        if (userId is null || name is null)
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

        await bus.PublishAsync(new Contracts.QuestionCreated(
            QuestionId: question.Id,
            Title: question.Title,
            Content: question.Content,
            Created: question.CreatedAt,
            Tags: question.TagSlugs
        ));

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
        var question = await db.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

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

        await bus.PublishAsync(new Contracts.QuestionDeleted(QuestionId: question.Id));

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

        if (!await tagService.AreTagsValidAsync(dto.Tags))
        {
            return BadRequest("One or more tags are invalid.");
        }


        question.Title = dto.Title;
        question.Content = dto.Content;
        question.TagSlugs = dto.Tags;
        question.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await bus.PublishAsync(new Contracts.QuestionUpdated(
            QuestionId: question.Id,
            Title: question.Title,
            Content: question.Content,
            Tags: question.TagSlugs.AsArray()
        ));

        return NoContent();
    }

    [Authorize]
    [HttpPost("{questionId}/answers")]
    public async Task<ActionResult<Answer>> AddAnswer(string questionId, CreateAnswerDto dto)
    {
        var question = await db.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId);
        if (question == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");
        if (userId is null || name is null)
            return BadRequest("User information is missing from the token.");

        var answer = new Answer
        {
            Content = dto.Content,
            UserDisplayName = name,
            UserId = userId,
            QuestionId = questionId
        };

        question.Answers.Add(answer);
        question.AnswersCount = question.Answers.Count;

        await db.SaveChangesAsync();

        await bus.PublishAsync(new Contracts.AnswerCountUpdated(
            QuestionId: questionId,
            AnswerCount: question.AnswersCount
        ));

        return Created($"/question/{questionId}/answers/{answer.Id}", answer);
    }

    [Authorize]
    [HttpPut("{questionId}/answers/{answerId}")]
    public async Task<IActionResult> UpdateAnswer(string questionId, string answerId, CreateAnswerDto dto)
    {
        var question = await db.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            return NotFound();
        }
        var answer = question.Answers.FirstOrDefault(a => a.Id == answerId);
        if (answer == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (answer.UserId != userId)
        {
            return Forbid();
        }

        answer.Content = dto.Content;
        answer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{questionId}/answers/{answerId}")]
    public async Task<IActionResult> DeleteAnswer(string questionId, string answerId)
    {
        var question = await db.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            return NotFound();
        }
        var answer = question.Answers.FirstOrDefault(a => a.Id == answerId);
        if (answer == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (answer.UserId != userId)
        {
            return Forbid();
        }
        question.Answers.Remove(answer);
        question.AnswersCount = question.Answers.Count;

        await db.SaveChangesAsync();

        await bus.PublishAsync(new Contracts.AnswerCountUpdated(
            QuestionId: questionId,
            AnswerCount: question.Answers.Count
        ));
        
        return NoContent();
    }

    [Authorize]
    [HttpPost("{questionId}/answers/{answerId}/accept")]
    public async Task<IActionResult> AcceptAnswer(string questionId, string answerId)
    {
        var question = await db.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            return NotFound();
        }

        var answer = question.Answers.FirstOrDefault(a => a.Id == answerId);

        if (answer == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (question.AskerId != userId)
        {
            return Forbid();
        }

        question.HasAcceptedAnswer = true;
        answer.Accepted = true;
        await db.SaveChangesAsync();

        await bus.PublishAsync(new Contracts.AnswerAccepted(
            QuestionId: questionId
        ));

        return NoContent();
    }
}