using System.Text.Json.Nodes;

namespace ProductsupConnector.Commands;

public class ExportCommand
{
    // Replace with your third-party API endpoint
    private const string ChannelApiEndpoint = "https://api.example.com/products";

    private readonly ContainerApiClient _client;

    public ExportCommand(ContainerApiClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        var totalProducts = await _client.Count(Files.Input);
        await _client.LogAndNotify(NotificationLevel.Info, $"Starting export of {totalProducts} products to channel.");

        var errorCount = 0;

        while (true)
        {
            var products = await _client.NextBatch(Files.Input);
            if (products.Count == 0)
                break;

            foreach (var product in products)
            {
                var id = product["id"]?.ToString();

                try
                {
                    await SendToChannel(product);
                }
                catch (Exception ex)
                {
                    errorCount++;

                    await _client.AppendToFeedback(new { id, status = "error", message = ex.Message });
                }
            }
        }

        if (errorCount > 0)
        {
            await _client.LogAndNotify(NotificationLevel.Warning, $"Export completed with {errorCount} errors out of {totalProducts} products.");
            return;
        }

        await _client.LogAndNotify(NotificationLevel.Success, $"Export completed: all {totalProducts} products exported successfully.");
    }

    /// <summary>
    /// Send a single product to the third-party channel.
    ///
    /// Replace this with your real API call — POST to a partner API, upload to a marketplace,
    /// push to an advertising platform, etc.
    /// </summary>
    private async Task SendToChannel(JsonNode product)
    {
        // Example: POST the product to the channel's API endpoint
        //
        // var response = await _httpClient.PostAsJsonAsync(ChannelApiEndpoint, new
        // {
        //     external_id = product["id"]?.ToString(),
        //     title = product["name"]?.ToString(),
        //     price = product["price"]?.ToString(),
        // });
        //
        // if (response.StatusCode != HttpStatusCode.OK)
        //     throw new Exception($"Channel API returned status {(int)response.StatusCode}");

        await Task.CompletedTask;
    }
}
