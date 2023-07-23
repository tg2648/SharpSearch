using System.CommandLine;
using SharpSearch.Indices;
using SharpSearch.Models;
using SharpSearch.Utilities;

namespace SharpSearch.Commands;

public class QueryCommand : ICommand
{
    public Command Command { get; }

    public QueryCommand(IIndex index)
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
            Stopwatcher.Time(() =>
            {
                IEnumerable<DocumentScore> results = index.CalculateDocumentScores(query);
                QueryResultPrinter.PrintResults(query, results, nOption);
            }, "Queried index in");
        }, queryArgument, nOption);
    }
}