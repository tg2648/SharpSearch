using System.CommandLine;
using SharpSearch.Commands;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");

        string indexFileName = "index.json";
        Index index = Stopwatcher.Time<Index>(() => new(indexFileName), "Loaded index in");

        foreach (var command in new CommandRepository(index).Commands)
        {
            rootCommand.AddCommand(command);
        }

        return await rootCommand.InvokeAsync(args);
    }
}
