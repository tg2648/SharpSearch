using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            foreach (string line in File.ReadLines(args[0]))
            {
                Console.WriteLine(line);
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("File could not be read:");
            Console.WriteLine(e.Message);
        }
    }
}
