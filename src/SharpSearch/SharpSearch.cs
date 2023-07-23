using System.CommandLine;
using SharpSearch.Commands;
using SharpSearch.Indices;
using SharpSearch.Models;
using JsonIndex = SharpSearch.Indices.JsonIndex;

namespace SharpSearch;

public class SharpSearch
{
    private const string INDEX_NAME = "index.json";
    private const string INDEX_DIR = "SharpSearch";

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");
        try
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string indexPath = Path.Combine(appDataFolder, INDEX_DIR, INDEX_NAME);
            IIndex index = Stopwatcher.Time<JsonIndex>(() => new(indexPath), "Loaded index in");
            IModel model = new TfIdfModel();

            index.Model = model;
            model.Index = index;

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
