using System;
using SearchService.Models;
using Typesense;

namespace SearchService.MessageHandlers;

public class QuestionDeletedHandler(ITypesenseClient client, ILogger<QuestionDeletedHandler> logger)
{
    public async Task HandleAsync(Contracts.QuestionDeleted message)
    {
        await client.DeleteDocument<SearchQuestion>("questions", message.QuestionId);
        
        logger.LogInformation($"Deleted question {message.QuestionId}");
    }
}
