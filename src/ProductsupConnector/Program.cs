using ProductsupConnector.ContainerApi;
using Refit;

var baseUrl = Environment.GetEnvironmentVariable("CONTAINER_API_URL") ?? "http://localhost:8080";
var client = RestService.For<IContainerApi>(new HttpClient { BaseAddress = new Uri(baseUrl) });

Console.WriteLine("Hello, Productsup!");
