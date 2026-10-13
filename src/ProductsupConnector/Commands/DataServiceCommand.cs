using ProductsupConnector.ContainerApi;

namespace ProductsupConnector.Commands;

public class DataServiceCommand
{
    private readonly ContainerApiClient _client;

    public DataServiceCommand(ContainerApiClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        var count = 0;
        while (true)
        {
            var products = await _client.NextBatch();
            if (products.Count == 0)
                break;

            foreach (var product in products)
                product["modified_by_dataservice"] = DateTime.UtcNow.ToString("o");

            await _client.WriteOutputBatch(products);
            count += products.Count;
        }

        await _client.Log(Level.Info, $"Finished data service. Products: {count}.");
    }
}
