using System.CommandLine;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch File Index");

        var addCommand = new Command("add", "Add files or directories to the index");
        var pathsArgument = new Argument<string[]>(
            name: "paths",
            description: "At least one path of file or directory to add to the index.")
        {
            Arity = ArgumentArity.OneOrMore
        };
        addCommand.AddArgument(pathsArgument);
        rootCommand.AddCommand(addCommand);

        var infoCommand = new Command("info", "Information about indexed files");
        rootCommand.AddCommand(infoCommand);

        string indexFileName = "index.json";
        Index index = new(indexFileName);

        addCommand.SetHandler((paths) =>
        {
            foreach (var path in paths)
            {
                Stopwatcher.Time(() => index.Add(path), "Added file(s) to index in");
            }
            index.Save();
        }, pathsArgument);

        infoCommand.SetHandler(() => index.Info());
        return await rootCommand.InvokeAsync(args);
    }
}
