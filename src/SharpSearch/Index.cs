using SharpSearch.Importers;

namespace SharpSearch;

class Index
{
    private Dictionary<string, IFileImporter> _extensionToImporter = new();

    public Index()
    {
        _extensionToImporter.Add(".txt", new TextImporter());
    }

    /// <summary>
    /// Recursively adds all files in a directory to the index
    /// </summary>
    private void AddDirectory(string dirPath)
    {
        foreach (string filePath in Directory.EnumerateFiles(dirPath, "*", SearchOption.AllDirectories))
        {
            AddFile(filePath);
        }
    }

    /// <summary>
    /// Adds file to the index
    /// </summary>
    private void AddFile(string filePath)
    {
        var file = new FileInfo(filePath);
        string extension = file.Extension;

        if (!_extensionToImporter.ContainsKey(extension))
        {
            Console.WriteLine($"SKIPPED: Unknown extension: {filePath}");
        }
        else
        {
            IFileImporter importer = _extensionToImporter[extension];
            var tokens = String.Join(", ", importer.ExtractTokens(file).Select(tok => $"<{tok}>"));
            Console.WriteLine($"File tokens: {tokens}");
        }
    }

    /// <summary>
    /// Adds provided file or directory of files into the index
    /// </summary>
    public void Add(string path)
    {
        if (File.Exists(path))
        {
            // This path is a file
            AddFile(path);
        }
        else if (Directory.Exists(path))
        {
            // This path is a directory
            AddDirectory(path);
        }
        else
        {
            Console.WriteLine("{0} is not a valid file or directory.", path);
        }
    }
}
