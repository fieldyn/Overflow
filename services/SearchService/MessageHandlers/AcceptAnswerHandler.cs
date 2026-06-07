using Typesense;

namespace SearchService.MessageHandlers;

public class AcceptAnswerHandler
{
    public async Task HandleAsync(
        Contracts.AnswerAccepted message,
        ITypesenseClient client,
        ILogger<AcceptAnswerHandler> logger)
    {
        await client.UpdateDocument("questions", message.QuestionId, new {
            HasAcceptedAnswer = true
        });

        logger.LogInformation($"Marked question {message.QuestionId} as having an accepted answer");
    }
}
