using System.CommandLine;

namespace SharpSearch.Commands;

class QueryCommand : ICommand
{
    public Command Command { get; }

    public QueryCommand(Index index)
    {
        Command = new Command("query", "Return documents in the index best matching the query");
        var queryArgument = new Argument<string>(name: "query", description: "Query terms to search for");
        var nOption = new Option<int>
            (name: "--n",
            description: "Number of documents to return.",
            getDefaultValue: () => 10);
        Command.AddArgument(queryArgument);
        Command.AddOption(nOption);
        Command.SetHandler((query, nOption) =>
        {
            Stopwatcher.Time(() => index.Query(query, nOption), "Queried the index in");
        }, queryArgument, nOption);
    }
}