using System.CommandLine;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var addCommand = new Command("add", "Add files or directories to the index");
        var pathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path of file or directory to add to the index.");
        pathsArgument.Arity = ArgumentArity.OneOrMore;
        addCommand.AddArgument(pathsArgument);

        Index index = new();

        addCommand.SetHandler((paths) =>
        {
            foreach (var path in paths)
            {
                index.Add(path);
            }
        }, pathsArgument);

        var rootCommand = new RootCommand("SharpSearch File Index");
        rootCommand.AddCommand(addCommand);
        return await rootCommand.InvokeAsync(args);
    }
}
