using ProductsupConnector.ContainerApi;

namespace ProductsupConnector.Commands;

public class ExportDeltaCommand
{
    private readonly ContainerApiClient _client;

    public ExportDeltaCommand(ContainerApiClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        var counts = new Dictionary<string, int>();
        foreach (var file in new[] { Files.New, Files.Modified, Files.Unchanged, Files.Deleted })
        {
            counts[file] = 0;
            while (true)
            {
                var products = await _client.NextBatch(file);
                if (products.Count == 0)
                    break;

                counts[file] += products.Count;
            }
        }

        await _client.Log(Level.Info,
            $"Finished export delta. New: {counts[Files.New]}, modified: {counts[Files.Modified]}, " +
            $"unchanged: {counts[Files.Unchanged]}, deleted: {counts[Files.Deleted]}.");
    }
}
