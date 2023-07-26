using System.CommandLine;
using SharpSearch.Indices;

namespace SharpSearch.Commands;

public class CommandRepository
{
    public IList<Command> Commands { get; }

    public CommandRepository(IIndex index)
    {
        Commands = new List<Command>()
        {
            new AddCommand(index).Command,
            new RemoveCommand(index).Command,
            new InfoCommand(index).Command,
            new QueryCommand(index).Command,
            new PruneCommand(index).Command,
        };
    }
}