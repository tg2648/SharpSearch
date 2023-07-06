using System.CommandLine;

namespace SharpSearch.Commands;

class InfoCommand : ICommand
{
    public Command Command { get; }

    public InfoCommand(Index index)
    {
        Command = new Command("info", "Return index statistics");
        Command.SetHandler(() => {
            Console.WriteLine(index.GetInfo());
        });
    }
}