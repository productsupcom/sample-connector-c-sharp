using System.Text.Json.Nodes;
using ProductsupConnector.ContainerApi;

namespace ProductsupConnector.Commands;

public class ExportCommand
{
    private readonly ContainerApiClient _client;

    public ExportCommand(ContainerApiClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        var count = 0;
        await foreach (var product in _client.ReadFile(Files.Input))
        {
            count++;
        }

        await _client.AppendToFeedback(new JsonObject
        {
            ["status"] = "success",
            ["message"] = "Finished export.",
        });
        await _client.Log(Level.Info, $"Finished export. Products: {count}.");
    }
}
