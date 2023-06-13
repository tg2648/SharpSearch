using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var importer = new TextImporter();
            var file = new FileInfo(args[0]);
            if (file.Exists)
            {
                foreach (String line in importer.extractLines(file))
                {
                    Console.WriteLine(line);
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("File could not be read:");
            Console.WriteLine(e.Message);
        }
    }
}
