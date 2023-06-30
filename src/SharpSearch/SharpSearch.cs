using System.CommandLine;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");

        string indexFileName = "index.json";
        Index index = new(indexFileName);

        /* ADD */
        var addCommand = new Command("add", "Add files or directories to the index.");
        var pathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path to file or directory to add to the index.")
        {
            Arity = ArgumentArity.OneOrMore
        };
        addCommand.AddArgument(pathsArgument);
        addCommand.SetHandler((paths) =>
        {
            foreach (var path in paths)
            {
                Stopwatcher.Time(() => index.Add(path), "Added file(s) to index in");
            }
            index.Save();
        }, pathsArgument);
        rootCommand.AddCommand(addCommand);

        /* QUERY */
        var queryCommand = new Command("query", "Return documents in the index best matching the query.");
        var queryArgument = new Argument<string>(name: "query");
        var nOption = new Option<int>
            (name: "--n",
            description: "Number of documents to return.",
            getDefaultValue: () => 42);
        queryCommand.AddArgument(queryArgument);
        queryCommand.AddOption(nOption);
        queryCommand.SetHandler((query, nOption) =>
        {
            Stopwatcher.Time(() => index.Query(query, nOption), "Queried the index in");
        }, queryArgument, nOption);
        rootCommand.AddCommand(queryCommand);

        /* INFO */
        var infoCommand = new Command("info", "Return index statistics.");
        infoCommand.SetHandler(index.Info);
        rootCommand.AddCommand(infoCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
