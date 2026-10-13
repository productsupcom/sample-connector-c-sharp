using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;
using Refit;

namespace ProductsupConnector;

public static class Files
{
    public const string Input = "input";
    public const string New = "new";
    public const string Modified = "modified";
    public const string Unchanged = "unchanged";
    public const string Deleted = "deleted";
    public const string Output = "output";
    public const string Feedback = "feedback";
}

public class ContainerApiClient
{
    private readonly IContainerApi _api;

    public ContainerApiClient(IContainerApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Fetches the next batch of products from the given file. Returns an empty list when there are no more items (204 No Content).
    /// </summary>
    public async Task<List<JsonNode>> NextBatch(string type = Files.Input, int batchSize = 100)
    {
        Response30 response;
        try
        {
            response = await _api.NextBatch(type, batchSize, null, null);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return [];
        }

        if (response.Data is null || response.Data.Count == 0)
            return [];

        var products = new List<JsonNode>(response.Data.Count);
        foreach (var item in response.Data)
        {
            if (item is JsonElement jsonElement
                && jsonElement.ValueKind != JsonValueKind.Null
                && jsonElement.ValueKind != JsonValueKind.Undefined)
            {
                var node = JsonNode.Parse(jsonElement.GetRawText());
                if (node is not null)
                    products.Add(node);
            }
        }

        return products;
    }

    /// <summary>
    /// Resets the cursor of the given file, so the next batch starts from the beginning.
    /// </summary>
    public async Task RestartCursor(string type)
    {
        await _api.Restart(type);
    }

    /// <summary>
    /// Reads all products from the given file, starting from the beginning.
    /// </summary>
    public async IAsyncEnumerable<JsonNode> ReadFile(
        string type,
        int batchSize = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await RestartCursor(type);

        while (!cancellationToken.IsCancellationRequested)
        {
            var products = await NextBatch(type, batchSize);
            if (products.Count == 0)
                yield break;

            foreach (var product in products)
                yield return product;
        }
    }

    public async Task Log(Level level, string message)
    {
        await _api.Logs(level, new Add_log_input(context: new { }, message: message));
    }

    public async Task Notify(Level2 level, string message)
    {
        await _api.Notifications(level, new Add_notification_input(context: new { }, message: message));
    }

    public async Task WriteOutputBatch(List<JsonNode> products)
    {
        await WriteBatch(Files.Output, products);
    }

    public async Task AppendToFeedback(JsonNode item)
    {
        await WriteBatch(Files.Feedback, [item]);
    }

    private async Task WriteBatch(string type, List<JsonNode> items)
    {
        var data = items
            .Select(p => JsonSerializer.Deserialize<object>(p.ToJsonString())!)
            .ToList<object>();

        var body = new Write_to_output_file_input(
            data: data,
            meta: new Meta(separator: ",")
        );
        await _api.OutputPost(type, body);
    }
}
