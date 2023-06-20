using SharpSearch.Importers;

namespace SharpSearch;

using TermFreq = System.Collections.Generic.Dictionary<DocumentId, int>;

class Index
{
    private Dictionary<string, IFileImporter> _extensionToImporter = new();

    // Inverted mapping of term -> document -> term frequency in that document
    private Dictionary<TermId, TermFreq> _termDocFreq = new();

    private HashSet<string> _indexedFiles = new();

    public Index()
    {
        _extensionToImporter.Add(".txt", new TextImporter());
        _extensionToImporter.Add(".md", new TextImporter());
        _extensionToImporter.Add(".html", new HTMLImporter());
        _extensionToImporter.Add(".xhtml", new HTMLImporter());
    }

    /// <summary>
    /// Recursively adds all files in a directory to the index
    /// </summary>
    private void AddDirectory(DirectoryInfo dir)
    {
        foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            AddFile(file);
        }
    }

    /// <summary>
    /// Adds file to the index
    /// </summary>
    private void AddFile(FileInfo file)
    {
        IFileImporter importer = _extensionToImporter[file.Extension];

        var tokenGroups = importer.ExtractTokens(file).GroupBy(token => token);

        Console.WriteLine($"Indexed: {file.FullName}");

        foreach (var group in tokenGroups)
        {
            string token = group.Key;
            _termDocFreq.TryAdd(token, new TermFreq());
            _termDocFreq[token].Add(file.FullName, group.Count());
        }

        _indexedFiles.Add(file.FullName);
    }

    /// <summary>
    /// Adds provided file or directory of files into the index
    /// </summary>
    public void Add(string path)
    {
        if (File.Exists(path))
        {
            // This path is a file
            var file = new FileInfo(path);
            string extension = file.Extension;

            if (!_extensionToImporter.ContainsKey(extension))
            {
                Console.WriteLine($"SKIPPED: Unknown extension {extension}: {path}");
            }
            else
            {
                AddFile(file);
            }
        }
        else if (Directory.Exists(path))
        {
            // This path is a directory
            var dir = new DirectoryInfo(path);
            AddDirectory(dir);
        }
        else
        {
            Console.WriteLine("{0} is not a valid file or directory.", path);
        }
    }

    public void Info()
    {
        Console.WriteLine("Indexed files:");

        foreach (var (term, tf) in _termDocFreq)
        {
            Console.WriteLine(term);
            foreach (var (doc, freq) in tf)
            {
                Console.WriteLine($"  {doc}: {freq}");
            }
        }
    }
}
