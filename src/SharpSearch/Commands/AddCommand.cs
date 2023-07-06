using System.CommandLine;

namespace SharpSearch.Commands;

class AddCommand : ICommand
{
    public Command Command { get; }

    public AddCommand(Index index)
    {
        Command = new Command("add", "Add files or directories to the index");
        var addPathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path to file or directory to add to the index")
        {
            Arity = ArgumentArity.OneOrMore
        };
        Command.AddArgument(addPathsArgument);
        Command.SetHandler((paths) =>
        {
            Stopwatcher.Time(() => {
                foreach (var path in paths)
                {
                    try
                    {
                        index.Add(path);
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
            }, "Added to index in");
            index.Save();
        }, addPathsArgument);
    }
}