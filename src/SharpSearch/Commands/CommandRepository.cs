using System.CommandLine;

namespace SharpSearch.Commands;

class CommandRepository
{
    public IList<Command> Commands { get; }

    public CommandRepository(Index index)
    {
        Commands = new List<Command>()
        {
            new AddCommand(index).Command,
            new RemoveCommand(index).Command,
            new InfoCommand(index).Command,
            new QueryCommand(index).Command,
        };
    }
}