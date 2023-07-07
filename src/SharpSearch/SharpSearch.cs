﻿using System.CommandLine;
using SharpSearch.Commands;

namespace SharpSearch;

public class SharpSearch
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SharpSearch Local Search Engine");
        try
        {
            Index index = Stopwatcher.Time<Index>(() => new(), "Loaded index in");

            foreach (var command in new CommandRepository(index).Commands)
            {
                rootCommand.AddCommand(command);
            }

            return await rootCommand.InvokeAsync(args);
        }
        catch (ApplicationException e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return -1;
        }
    }
}
