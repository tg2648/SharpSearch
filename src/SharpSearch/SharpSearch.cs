using System.CommandLine;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");

        string indexFileName = "index.json";
        Index index = Stopwatcher.Time<Index>(() => new(indexFileName), "Loaded index in");

        /* ADD */
        var addCommand = new Command("add", "Add files or directories to the index");
        var addPathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path to file or directory to add to the index")
        {
            Arity = ArgumentArity.OneOrMore
        };
        addCommand.AddArgument(addPathsArgument);
        addCommand.SetHandler((paths) =>
        {
            Stopwatcher.Time(() => {
                foreach (var path in paths)
                {
                    index.Add(path);
                }
            }, "Added to index in");
            index.Save();
        }, addPathsArgument);
        rootCommand.AddCommand(addCommand);

        /* REMOVE */
        var removeCommand = new Command("remove", "Remove file or directory from the index");
        var removePathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path to file or directory to remove from the index")
        {
            Arity = ArgumentArity.OneOrMore
        };
        removeCommand.AddArgument(removePathsArgument);
        removeCommand.SetHandler((paths) =>
        {
            Stopwatcher.Time(() => {
                foreach (var path in paths)
                {
                    index.Remove(path);
                }
            }, "Removed from index in");
            index.Save();
        }, removePathsArgument);
        rootCommand.AddCommand(removeCommand);

        /* QUERY */
        var queryCommand = new Command("query", "Return documents in the index best matching the query");
        var queryArgument = new Argument<string>(name: "query", description: "Query terms to search for");
        var nOption = new Option<int>
            (name: "--n",
            description: "Number of documents to return.",
            getDefaultValue: () => 10);
        queryCommand.AddArgument(queryArgument);
        queryCommand.AddOption(nOption);
        queryCommand.SetHandler((query, nOption) =>
        {
            Stopwatcher.Time(() => index.Query(query, nOption), "Queried the index in");
        }, queryArgument, nOption);
        rootCommand.AddCommand(queryCommand);

        /* INFO */
        var infoCommand = new Command("info", "Return index statistics");
        infoCommand.SetHandler(index.Info);
        rootCommand.AddCommand(infoCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
