using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;

namespace ProductsupConnector.Commands;

public class DataSourceCommand
{
    private readonly ContainerApiClient _client;

    public DataSourceCommand(ContainerApiClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        var totalProducts = int.TryParse(Environment.GetEnvironmentVariable("PRODUCTS"), out var p) ? p : 1000;
        var batchSize = int.TryParse(Environment.GetEnvironmentVariable("BATCH"), out var b) && b > 0 ? b : 100;

        for (var i = 0; i < totalProducts; i += batchSize)
        {
            var products = Enumerable
                .Range(i, Math.Min(batchSize, totalProducts - i))
                .Select(GenerateProduct)
                .ToList();

            await _client.WriteOutputBatch(products);
        }

        await _client.Log(Level.Info, $"Finished datasource. Products: {totalProducts}.");
    }

    private static JsonNode GenerateProduct(int index)
    {
        var now = DateTime.UtcNow.ToString("o");

        return new JsonObject
        {
            ["id"] = Guid.NewGuid().ToString(),
            ["category"] = $"Category {Random.Shared.Next(1, 20)}",
            ["name"] = $"Product {index + 1}",
            ["description"] = $"Description of product {index + 1}",
            ["price"] = Math.Round(Random.Shared.NextDouble() * 1000, 2),
            ["url"] = $"https://example.com/products/{index + 1}",
            ["quantity"] = Random.Shared.Next(1, 101),
            ["image"] = $"https://example.com/images/{index + 1}.jpg",
            ["created_at"] = now,
            ["updated_at"] = now,
        };
    }
}
