using ProductsupConnector;
using ProductsupConnector.Commands;
using ProductsupConnector.ContainerApi;
using Refit;

var baseUrl = Environment.GetEnvironmentVariable("CONTAINER_API_URL") ?? "http://cde-container-api";
var api = RestService.For<IContainerApi>(new HttpClient { BaseAddress = new Uri(baseUrl) });
var client = new ContainerApiClient(api);

// Usage: dotnet ProductsupConnector.dll [datasource|data-service|export|export-delta]
var command = args.Length > 0 ? args[0] : "data-service";

try
{
    switch (command)
    {
        case "datasource":
            await new DataSourceCommand(client).Run();
            break;
        case "data-service":
            await new DataServiceCommand(client).Run();
            break;
        case "export":
            await new ExportCommand(client).Run();
            break;
        case "export-delta":
            await new ExportDeltaCommand(client).Run();
            break;
        default:
            Console.Error.WriteLine($"Unknown command '{command}'. Available: datasource, data-service, export, export-delta.");
            return 1;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

return 0;

