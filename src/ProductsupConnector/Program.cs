using System.Text.Json;
using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;
using Refit;

var baseUrl = Environment.GetEnvironmentVariable("CONTAINER_API_URL") ?? "http://cde-container-api";
var client = RestService.For<IContainerApi>(new HttpClient { BaseAddress = new Uri(baseUrl) });

while (true)
{
    var response = await client.Next("input", null, null);

    if (response.Data is null)
        break;

    var jsonElement = (JsonElement)response.Data;
    if (jsonElement.ValueKind == JsonValueKind.Null || jsonElement.ValueKind == JsonValueKind.Undefined)
        break;

    var product = JsonNode.Parse(jsonElement.GetRawText());
    if (product is null)
        break;

    // Log the product as JSON at info level
    var logMessage = product.ToJsonString();
    await client.Logs(Level.Info, new Add_log_input(context: new { }, message: logMessage));

    // Add modified_by_dataservice property with current datetime
    product["modified_by_dataservice"] = DateTime.UtcNow.ToString("o");

    // Write the product to output
    var outputBody = new Write_to_output_file_input(
        data: new List<object> { JsonSerializer.Deserialize<object>(product.ToJsonString())! },
        meta: new Meta(separator: ",")
    );
    await client.OutputPost("output", outputBody);
}
