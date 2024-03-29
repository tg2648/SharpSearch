using System.CommandLine;
using SharpSearch.Indices;

namespace SharpSearch.Commands;

public class RemoveCommand : ICommand
{
    public Command Command { get; }

    public RemoveCommand(IIndex index)
    {
        Command = new Command("remove", "Remove file or directory from the index");
        var removePathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path to file or directory to remove from the index")
        {
            Arity = ArgumentArity.OneOrMore
        };
        Command.AddArgument(removePathsArgument);
        Command.SetHandler((paths) =>
        {
            Stopwatcher.Time(() =>
            {
                foreach (var path in paths)
                {
                    try
                    {
                        index.Remove(path);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"ERROR: {e.Message}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: {e.Message}");
                    }
                }
            }, "Removed from index in");
        }, removePathsArgument);
    }
}