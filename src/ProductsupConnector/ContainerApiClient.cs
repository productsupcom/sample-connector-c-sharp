using System.Text.Json;
using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;

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
    /// Returns the number of products in the given file.
    /// </summary>
    public async Task<int> Count(string type = Files.Input)
    {
        var response = await _api.Count(type);
        return response.Data.Count ?? 0;
    }

    /// <summary>
    /// Fetches the next batch of products from the given file. Returns an empty list when there are no more items (204 No Content).
    /// </summary>
    public async Task<List<JsonNode>> NextBatch(string type = Files.Input, int batchSize = 100)
    {
        var response = await _api.NextBatch(type, batchSize, null, null);
        if (response?.Data is null)
            return [];

        // OfType skips null items
        return response.Data.OfType<JsonElement>().Select(item => JsonSerializer.SerializeToNode(item)!).ToList();
    }

    public async Task Log(LogLevel level, string message)
    {
        await _api.Logs(level, new Add_log_input(context: new { }, message: message));
    }

    public async Task Notify(NotificationLevel level, string message)
    {
        await _api.Notifications(level, new Add_notification_input(context: new { }, message: message));
    }

    /// <summary>
    /// Writes the message to the log and sends it to the user as a notification, both with the same level.
    /// </summary>
    public async Task LogAndNotify(NotificationLevel level, string message)
    {
        var logLevel = level switch
        {
            NotificationLevel.Notice => LogLevel.Notice,
            NotificationLevel.Warning => LogLevel.Warning,
            NotificationLevel.Error => LogLevel.Error,
            _ => LogLevel.Info, // Info, and Success which logs don't have
        };

        await Log(logLevel, message);
        await Notify(level, message);
    }

    public async Task WriteOutputBatch(List<JsonNode> products)
    {
        await WriteBatch(Files.Output, [.. products]);
    }

    public async Task AppendToFeedback(object item)
    {
        await WriteBatch(Files.Feedback, [item]);
    }

    private async Task WriteBatch(string type, ICollection<object> items)
    {
        await _api.OutputPost(type, new Write_to_output_file_input(data: items, meta: new Meta(separator: ",")));
    }
}
