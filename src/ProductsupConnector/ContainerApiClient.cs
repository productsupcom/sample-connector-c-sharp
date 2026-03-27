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
    /// Fetches the next product from input. Returns null when there are no more items (204 No Content).
    /// </summary>
    public async Task<JsonNode?> NextProduct()
    {
        Response34 response;
        try
        {
            response = await _api.Next("input", null, null);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return null;
        }

        if (response.Data is null)
            return null;

        var jsonElement = (JsonElement)response.Data;
        if (jsonElement.ValueKind == JsonValueKind.Null || jsonElement.ValueKind == JsonValueKind.Undefined)
            return null;

        return JsonNode.Parse(jsonElement.GetRawText());
    }

    public async Task Log(Level level, string message)
    {
        await _api.Logs(level, new Add_log_input(context: new { }, message: message));
    }

    public async Task WriteOutput(JsonNode product)
    {
        var outputBody = new Write_to_output_file_input(
            data: new List<object> { JsonSerializer.Deserialize<object>(product.ToJsonString())! },
            meta: new Meta(separator: ",")
        );
        await _api.OutputPost("output", outputBody);
    }
}
