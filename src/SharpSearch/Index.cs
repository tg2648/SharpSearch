using System.Text.Json;
using System.Text.Json.Nodes;
using SharpSearch.Importers;

namespace SharpSearch;

using TermDocFreq = System.Collections.Generic.Dictionary<string, int>;

class Index
{
    private Dictionary<string, IFileImporter> _extensionToImporter = new()
    {
        [".txt"] = new TextImporter(),
        [".md"] = new TextImporter(),
        [".html"] = new HTMLImporter(),
        [".xhtml"] = new HTMLImporter(),
    };

    // Inverted mapping of term -> document -> term frequency in that document
    private Dictionary<string, TermDocFreq> _terms = new();
    private Dictionary<string, string> _fileIds = new();
    private HashSet<string> _indexedFiles = new();

    public Index(string indexFileName)
    {
        string jsonString = File.ReadAllText(indexFileName);
        using (JsonDocument document = JsonDocument.Parse(jsonString))
        {
            JsonElement root = document.RootElement;
            JsonElement termsElement = root.GetProperty("_terms");
            _terms = JsonSerializer.Deserialize<Dictionary<string, TermDocFreq>>(termsElement)!;

            JsonElement fileIdsElement = root.GetProperty("_fileIds");
            _fileIds = JsonSerializer.Deserialize<Dictionary<string, string>>(fileIdsElement)!;
        }
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
        string fileId = Guid.NewGuid().ToString("n");
        _fileIds.Add(fileId, file.FullName);

        var tokenGroups = importer.ExtractTokens(file).GroupBy(token => token);
        foreach (var group in tokenGroups)
        {
            string token = group.Key;
            _terms.TryAdd(token, new TermDocFreq());
            _terms[token].Add(fileId, group.Count());
        }

        _indexedFiles.Add(file.FullName);
        Console.WriteLine($"Indexed: {file.FullName}");
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

        foreach (var (term, tf) in _terms)
        {
            Console.WriteLine(term);
            foreach (var (doc, freq) in tf)
            {
                Console.WriteLine($"  {doc}: {freq}");
            }
        }
    }

    public void Save()
    {
        string fileName = "index.json";
        string jsonString = JsonSerializer.Serialize(new { _terms = _terms, _fileIds = _fileIds });
        File.WriteAllText(fileName, jsonString);
    }
}
