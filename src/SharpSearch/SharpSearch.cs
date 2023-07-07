using System.CommandLine;
using SharpSearch.Commands;

namespace SharpSearch;

public class SharpSearch
{

    /// <summary>
    ///     Creates a blank index file if one does not exist in the ApplicationData folder.
    /// </summary>
    /// <returns>
    ///     Path to the index file
    /// </returns>
    private static string Initialize()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string indexPath = Path.Combine(appDataFolder, "SharpSearch", "index.json");

        if (!File.Exists(indexPath))
        {
            string? indexDir = Path.GetDirectoryName(indexPath);
            if (indexDir == null || indexDir == string.Empty)
            {
                throw new ApplicationException($"Cannot initialize index file in {indexPath}");
            }

            Directory.CreateDirectory(indexDir);
            FileStream indexFile = File.Create(indexPath);
            indexFile.Dispose();

            Console.WriteLine($"Initialized new index in {indexPath}");
        }

        return indexPath;
    }

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");
        try
        {
            string indexPath = Initialize();
            Index index = Stopwatcher.Time<Index>(() => new(indexPath), "Loaded index in");

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
