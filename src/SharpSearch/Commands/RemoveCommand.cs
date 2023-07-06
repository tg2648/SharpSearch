using System.CommandLine;

namespace SharpSearch.Commands;

class RemoveCommand : ICommand
{
    public Command Command { get; }

    public RemoveCommand(Index index)
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
            Stopwatcher.Time(() => {
                foreach (var path in paths)
                {
                    index.Remove(path);
                }
            }, "Removed from index in");
            index.Save();
        }, removePathsArgument);
    }
}