using System.Text.Json;
using SharpSearch.Importers;
using SharpSearch.Utilities;

namespace SharpSearch;

using TermDocFreq = Dictionary<string, int>;

class Index
{
    private readonly Dictionary<string, IFileImporter> _extensionToImporter = new()
    {
        [".txt"] = new TextImporter(),
        [".md"] = new TextImporter(),
        [".html"] = new HTMLImporter(),
        [".xhtml"] = new HTMLImporter(),
    };

    // Inverted mapping of term -> document -> term frequency in that document
    private readonly Dictionary<string, TermDocFreq> _terms = new();
    private readonly Dictionary<string, string> _filePaths = new(); // Id -> Path
    private readonly Dictionary<string, string> _fileIds = new(); // Path -> Id

    public Index(string indexFileName)
    {
        if (File.Exists(indexFileName))
        {
            string jsonString = File.ReadAllText(indexFileName);
            using JsonDocument document = JsonDocument.Parse(jsonString);

            JsonElement root = document.RootElement;
            JsonElement termsElement = root.GetProperty("_terms");
            _terms = JsonSerializer.Deserialize<Dictionary<string, TermDocFreq>>(termsElement)!;

            JsonElement filePathsElement = root.GetProperty("_filePaths");
            _filePaths = JsonSerializer.Deserialize<Dictionary<string, string>>(filePathsElement)!;

            foreach ((string fileId, string filePath) in _filePaths)
            {
                _fileIds.Add(filePath, fileId);
            }
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
        string extension = file.Extension;
        string path = file.FullName;

        if (!_extensionToImporter.ContainsKey(extension))
        {
            Console.WriteLine($"SKIPPED: Unknown extension {extension}: {path}");
        }
        else
        {
            string fileId;
            if (_fileIds.ContainsKey(path))
            {
                fileId = _fileIds[path];
            }
            else
            {
                fileId = Guid.NewGuid().ToString("n");
                _fileIds.Add(path, fileId);
                _filePaths.Add(fileId, path);
            }

            IFileImporter importer = _extensionToImporter[extension];
            var tokenGroups = importer.ExtractTokens(file).GroupBy(token => token);
            foreach (var group in tokenGroups)
            {
                string token = group.Key;
                _terms.TryAdd(token, new TermDocFreq());
                _terms[token][fileId] = group.Count();
            }

            Console.WriteLine($"Indexed: {path}");
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
            var file = new FileInfo(path);
            AddFile(file);
        }
        else if (Directory.Exists(path))
        {
            // This path is a directory
            var dir = new DirectoryInfo(path);
            AddDirectory(dir);
        }
        else
        {
            Console.WriteLine($"{path} is not a valid file or directory.");
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
        string jsonString = JsonSerializer.Serialize(new { _terms, _filePaths });
        File.WriteAllText(fileName, jsonString);
    }

    /// <summary>
    /// Returns top N documents matching the query
    /// </summary>
    public void Query(string query, int n = 10)
    {
    }
}
