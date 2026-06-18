using System;
using System.Linq;
using System.Text.RegularExpressions;
using SearchService.Models;
using Typesense;

namespace SearchService.MessageHandlers;

public class QuestionUpdatedHandler(ITypesenseClient client, ILogger<QuestionUpdatedHandler> logger)
{
    public async Task HandleAsync(Contracts.QuestionUpdated message)
    {
        await client.UpdateDocument("questions", message.QuestionId, new {
            title = message.Title,
            content = StripHtml(message.Content),
            tags = message.Tags.ToArray()
        });

        logger.LogInformation($"Updated question {message.QuestionId}");
    }

    private static string StripHtml(string content)
    {
        return Regex.Replace(content, "<.*?>", string.Empty);
    }
}
