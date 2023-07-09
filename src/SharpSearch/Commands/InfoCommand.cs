using System.CommandLine;
using SharpSearch.Indices;

namespace SharpSearch.Commands;

class InfoCommand : ICommand
{
    public Command Command { get; }

    public InfoCommand(IIndex index)
    {
        Command = new Command("info", "Return index statistics");
        Command.SetHandler(() => {
            Console.WriteLine($"There are {index.GetInfo().DocumentCount} documents in the index");
        });
    }
}