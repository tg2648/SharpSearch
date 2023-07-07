using System.Text;
using System.Text.Json;
using SharpSearch.Importers;
using SharpSearch.Utilities;

namespace SharpSearch;

using TermDocFreq = Dictionary<string, int>;

class Index
{
    private readonly string _indexPath;
    private readonly Dictionary<string, IFileImporter> _extensionToImporter = new()
    {
        [".txt"] = new TextImporter(),
        [".md"] = new TextImporter(),
        [".html"] = new HTMLImporter(),
        [".xhtml"] = new HTMLImporter(),
    };

    // Inverted mapping of term -> document -> term frequency in that document
    private readonly Dictionary<string, TermDocFreq> _terms = new();
    private readonly Dictionary<string, Document> _files = new(); // Id -> Document
    private readonly Dictionary<string, string> _fileIds = new(); // Path -> Id

    public Index(string indexPath)
    {
        _indexPath = indexPath;
        string jsonString = File.ReadAllText(indexPath);

        if (jsonString == string.Empty)
            return;

        try
        {
            using JsonDocument document = JsonDocument.Parse(jsonString);

            JsonElement root = document.RootElement;
            JsonElement termsElement = root.GetProperty(nameof(_terms));
            _terms = JsonSerializer.Deserialize<Dictionary<string, TermDocFreq>>(termsElement)!;

            JsonElement filePathsElement = root.GetProperty(nameof(_files));
            _files = JsonSerializer.Deserialize<Dictionary<string, Document>>(filePathsElement)!;

            foreach ((string fileId, Document doc) in _files)
            {
                _fileIds.Add(doc.Path, fileId);
            }
        }
        catch (Exception e) when (e is JsonException || e is KeyNotFoundException)
        {
            // Something went wrong with parsing/deserializing JSON
            throw new ApplicationException($"Index file located in {indexPath} is invalid. Please try re-creating the index.");
        }
    }

    /* Private Interface */

    /// <summary>
    ///     Recursively adds all files in a directory to the index
    /// </summary>
    private void AddDirectory(DirectoryInfo dir)
    {
        foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            AddFile(file);
        }
    }

    /// <summary>
    ///     Adds file to the index
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
            IFileImporter importer = _extensionToImporter[extension];
            IEnumerable<string> tokens = importer.ExtractTokens(file);
            int documentLength = tokens.Count();
            var tokenGroups = tokens.GroupBy(token => token);

            string fileId;
            if (_fileIds.ContainsKey(path))
            {
                fileId = _fileIds[path];
            }
            else
            {
                fileId = Guid.NewGuid().ToString("n");
                _fileIds.Add(path, fileId);
                _files.Add(fileId, new Document(path, documentLength, file.LastWriteTimeUtc));
            }

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
    ///     Recursively removes all files in a directory from the index
    /// </summary>
    /// <remarks>
    ///     DirectoryInfo is used to get absolute paths from FileInfo
    /// </remarks>
    private void RemoveDirectory(DirectoryInfo dir)
    {
        foreach (FileInfo file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            RemoveFile(file.FullName);
        }
    }

    /// <summary>
    ///     Removes file from the index
    /// </summary>
    private void RemoveFile(string path)
    {
        if (_fileIds.ContainsKey(path))
        {
            string id = _fileIds[path];
            _files.Remove(id);

            HashSet<string> emptyTerms = new();
            foreach ((string term, TermDocFreq tdf) in _terms)
            {
                tdf.Remove(id);
                if (tdf.Count == 0) emptyTerms.Add(term);
            }

            // Prune empty terms
            foreach (string term in emptyTerms)
            {
                _terms.Remove(term);
            }

            Console.WriteLine($"Removed from index: {path}");
        }
    }

    private double GetTf(string term, string docId)
    {
        int tf = _terms[term].GetValueOrDefault(docId, 0);
        // return tf / (double)_files[docId].Length;
        return Math.Log10(1 + tf);
    }

    private double GetIdf(string term)
    {
        int N = _files.Count;
        int df = _terms[term].Count;

        return Math.Log10(N / df);
    }

    private double CalculateTfIdf(string term, string docId)
    {
        return GetTf(term, docId) * GetIdf(term);
    }

    /// <summary>
    ///     Returns paths to top N documents matching the query
    /// </summary>
    private IEnumerable<string> RunQuery(string query, int n = 10)
    {
        var scores = new Dictionary<string, double>();

        foreach (string token in Tokenizer.ExtractTokens(query))
        {
            if (_terms.ContainsKey(token))
            {
                // Increment the score for each document that has the term
                foreach ((string docId, int termCount) in _terms[token])
                {
                    string docPath = _files[docId].Path;
                    if (!scores.ContainsKey(docPath))
                        scores[docPath] = 0;

                    scores[docPath] = scores[docPath] + CalculateTfIdf(token, docId);
                }
            }
        }

        // Length-normalize using document length
        foreach ((string docPath, double score) in scores)
        {
            string docId = _fileIds[docPath];
            scores[docPath] = scores[docPath] / _files[docId].Length;
        }

        double maxScore = scores.Count > 0 ? scores.Values.Max() : -1;

        var result = scores
        .OrderByDescending((kvp) => kvp.Value)
        .Take(n)
        .Select((kvp) => $"[{kvp.Value}] {kvp.Key}");
        // .Select((kvp) => $"[{Math.Round(kvp.Value / maxScore * 100d, 2)}%] {kvp.Key}");

        return result;
    }

    private static void PrintQueryResults(string query, IEnumerable<string> results)
    {
        Console.WriteLine($"Query results for \"{query}\" (including relative relevance):");

        int idx = 1;
        foreach (string result in results)
        {
            Console.WriteLine($"{idx++}. {result}");
        }
    }

    /* Public Interface */

    /// <summary>
    ///     Adds provided file or directory of files into the index
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
            throw new ArgumentException($"{path} is not a valid file or directory.");
        }
    }

    /// <summary>
    ///     Removes provided file or directory of files from the index
    /// </summary>
    public void Remove(string path)
    {
        if (File.Exists(path))
        {
            // This path is a file
            RemoveFile(path);
        }
        else if (Directory.Exists(path))
        {
            // This path is a directory
            var dir = new DirectoryInfo(path);
            RemoveDirectory(dir);
        }
        else
        {
            throw new ArgumentException($"{path} is not a valid file or directory.");
        }
    }

    /// <summary>
    ///     Returns text with statistics about the index
    /// </summary>
    public string GetInfo()
    {
        var info = new StringBuilder();
        info.AppendLine("Indexed files:");

        foreach (var (term, tf) in _terms)
        {
            info.AppendLine(term);
            foreach (var (doc, freq) in tf)
            {
                info.AppendLine($"  {doc}: {freq}");
            }
        }

        return info.ToString();
    }

    /// <summary>
    ///     Saves index to disk
    /// </summary>
    public void Save()
    {
        string jsonString = JsonSerializer.Serialize(new { _terms, _files });
        File.WriteAllText(_indexPath, jsonString);
    }

    /// <summary>
    ///     Prints paths to top N documents matching the query
    /// </summary>
    public void Query(string query, int n = 10)
    {
        IEnumerable<string> results = RunQuery(query, n);
        PrintQueryResults(query, results);
    }

    /// <summary>
    ///     Re-indexes any files in the index that have been modified
    ///     since they were last added to the index.
    /// </summary>
    public void Refresh()
    {
        throw new NotImplementedException();
    }
}
