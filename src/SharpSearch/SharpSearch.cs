namespace SharpSearch;

public class SharpSearch
{
    public static void Main(string[] args)
    {
        string path = args[0];
        Index index = new();
        index.Add(path);
    }
}
