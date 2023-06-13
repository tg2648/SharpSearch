using System;
using System.IO;

public class SharpSearch
{
    public static void Main(string[] args)
    {
        var importer = new TextImporter();
        var file = new FileInfo(args[0]);
        if (file.Exists)
        {
            foreach (String token in importer.ExtractTokens(file))
            {
                Console.Write($"{token} ");
            }
            Console.WriteLine();
        }
    }
}
