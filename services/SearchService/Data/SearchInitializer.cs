using Typesense;

namespace SearchService.Data;

public static class SearchInitializer
{
    public static async Task EnsureIndexExists(ITypesenseClient client)
    {
        const string schemaName = "questions";

        try
        {
            await client.RetrieveCollection(schemaName);
            Console.WriteLine($"Schema '{schemaName}' already exists.");
            return;
        }
        catch (TypesenseApiNotFoundException)
        {
            Console.WriteLine($"Schema '{schemaName}' does not exist. Creating schema...");
        }

        var schema = new Schema(
            schemaName,
            new List<Field>
            {
                new Field("id", FieldType.String, false),
                new Field("title", FieldType.String, false),
                new Field("content", FieldType.String, false),
                new Field("tags", FieldType.StringArray, true),
                new Field("createdAt", FieldType.Int64, false),
                new Field("hasAcceptedAnswer", FieldType.Bool, false),
                new Field("answerCount", FieldType.Int32, false),
            },
            "createdAt"
        );

        await client.CreateCollection(schema);
        Console.WriteLine($"Schema '{schemaName}' created successfully.");
    }
}
