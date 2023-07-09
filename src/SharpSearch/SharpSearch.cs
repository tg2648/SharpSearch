using System.CommandLine;
using SharpSearch.Commands;
using SharpSearch.Indices;
using SharpSearch.Models;
using JsonIndex = SharpSearch.Indices.JsonIndex;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");
        try
        {
            IIndex index = Stopwatcher.Time<JsonIndex>(() => new(), "Loaded index in");
            IModel model = new TfIdfModel();

            index.SetModel(model);
            model.SetIndex(index);

            foreach (var command in new CommandRepository(index).Commands)
            {
                rootCommand.AddCommand(command);
            }

            return await rootCommand.InvokeAsync(args);
        }
        catch (ApplicationException e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return -1;
        }
    }
}
