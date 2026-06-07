using Typesense;

namespace SearchService.MessageHandlers;

public class AnswerCountHandler
{
    public async Task HandleAsync(
        Contracts.AnswerCountUpdated message,
        ITypesenseClient client,
        ILogger<AnswerCountHandler> logger)
    {
        await client.UpdateDocument("questions", message.QuestionId, new {
            AnswerCount = message.AnswerCount
        });

        logger.LogInformation($"Updated answer count for question {message.QuestionId} to {message.AnswerCount}");
    }
}
