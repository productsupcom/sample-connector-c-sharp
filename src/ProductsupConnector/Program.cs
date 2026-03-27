using ProductsupConnector;
using ProductsupConnector.ContainerApi;
using Refit;

var baseUrl = Environment.GetEnvironmentVariable("CONTAINER_API_URL") ?? "http://cde-container-api";
var api = RestService.For<IContainerApi>(new HttpClient { BaseAddress = new Uri(baseUrl) });
var client = new ContainerApiClient(api);

while (true)
{
    var products = await client.NextBatch();
    if (products.Count == 0)
        break;

    foreach (var product in products)
    {
        await client.Log(Level.Info, product.ToJsonString());
        await client.Notify(Level2.Info, product.ToJsonString());
        product["modified_by_dataservice"] = DateTime.UtcNow.ToString("o");
    }

    await client.WriteOutputBatch(products);
}
