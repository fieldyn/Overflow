using Typesense;

namespace SearchService.MessageHandlers;

public class AcceptAnswerHandler(ITypesenseClient client, ILogger<AcceptAnswerHandler> logger)
{
    public async Task HandleAsync(Contracts.AnswerAccepted message)
    {
        await client.UpdateDocument("questions", message.QuestionId, new {
            hasAcceptedAnswer = true
        });

        logger.LogInformation($"Marked question {message.QuestionId} as having an accepted answer");
    }
}
