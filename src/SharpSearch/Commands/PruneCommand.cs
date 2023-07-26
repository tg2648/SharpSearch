using System.CommandLine;
using SharpSearch.Indices;

namespace SharpSearch.Commands;

public class PruneCommand : ICommand
{
    public Command Command { get; }

    public PruneCommand(IIndex index)
    {
        Command = new Command("prune", "Remove documents from the index that no longer exist");
        Command.SetHandler(() =>
        {
            Console.WriteLine($"Pruned {index.Prune()} documents from the index that no longer exist.");
        });
    }
}