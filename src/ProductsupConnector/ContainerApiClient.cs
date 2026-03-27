using System.Text.Json;
using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;
using Refit;

namespace ProductsupConnector;

public class ContainerApiClient
{
    private readonly IContainerApi _api;

    public ContainerApiClient(IContainerApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Fetches the next batch of products from input. Returns an empty list when there are no more items (204 No Content).
    /// </summary>
    public async Task<List<JsonNode>> NextBatch(int batchSize = 100)
    {
        Response30 response;
        try
        {
            response = await _api.NextBatch("input", batchSize, null, null);
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
        var data = products
            .Select(p => JsonSerializer.Deserialize<object>(p.ToJsonString())!)
            .ToList<object>();

        var outputBody = new Write_to_output_file_input(
            data: data,
            meta: new Meta(separator: ",")
        );
        await _api.OutputPost("output", outputBody);
    }
}
