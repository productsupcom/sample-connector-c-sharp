using ProductsupConnector;
using ProductsupConnector.ContainerApi;
using Refit;

var baseUrl = Environment.GetEnvironmentVariable("CONTAINER_API_URL") ?? "http://cde-container-api";
var api = RestService.For<IContainerApi>(new HttpClient { BaseAddress = new Uri(baseUrl) });
var client = new ContainerApiClient(api);

while (true)
{
    var product = await client.NextProduct();
    if (product is null)
        break;

    await client.Log(Level.Info, product.ToJsonString());

    product["modified_by_dataservice"] = DateTime.UtcNow.ToString("o");

    await client.WriteOutput(product);
}
