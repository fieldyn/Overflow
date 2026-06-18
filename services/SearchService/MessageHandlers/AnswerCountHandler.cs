using Typesense;

namespace SearchService.MessageHandlers;

public class AnswerCountHandler(ITypesenseClient client, ILogger<AnswerCountHandler> logger)
{
    public async Task HandleAsync(Contracts.AnswerCountUpdated message)
    {
        await client.UpdateDocument("questions", message.QuestionId, new {
            answerCount = message.AnswerCount
        });

        logger.LogInformation($"Updated answer count for question {message.QuestionId} to {message.AnswerCount}");
    }
}
