using System.Text;
using System.Text.Json;
using SharpSearch.Importers;
using SharpSearch.Models;
using SharpSearch.Utilities;

namespace SharpSearch.Indices;

using TermDocFreq = Dictionary<string, int>;

public class JsonIndex : IIndex
{
    private readonly string _indexPath;
    public IModel? Model { get; set; }

    private readonly Dictionary<string, IFileImporter> _extensionToImporter = new()
    {
        [".txt"] = new TextImporter(),
        [".md"] = new TextImporter(),
        [".html"] = new HTMLImporter(),
        [".xhtml"] = new HTMLImporter(),
    };

    // Inverted mapping of term -> document -> term frequency in that document
    private Dictionary<string, TermDocFreq> _terms = new();
    private Dictionary<string, Document> _files = new(); // Id -> Document
    private readonly Dictionary<string, string> _fileIds = new(); // Path -> Id


    public JsonIndex(string indexPath)
    {
        // Use existing index or initialize a new one
        _indexPath = indexPath;
        Initialize(_indexPath);
        ParseIndexFile(_indexPath);
    }

    /* Private Interface */

    /// <summary>
    ///     Creates a blank index file if one does not exist in the ApplicationData folder.
    /// </summary>
    /// <returns>
    ///     Path to the index file
    /// </returns>
    private static string Initialize(string indexPath)
    {
        if (!File.Exists(indexPath))
        {
            string? indexDir = Path.GetDirectoryName(indexPath);
            Console.WriteLine($"Existing index not found in {indexDir}");

            if (indexDir == null || indexDir == string.Empty)
            {
                throw new ApplicationException($"Cannot initialize index file in {indexPath}");
            }

            Directory.CreateDirectory(indexDir);
            FileStream indexFile = File.Create(indexPath);
            indexFile.Dispose();

            Console.WriteLine($"Initialized new index in {indexPath}");
        }

        return indexPath;
    }

    private void ParseIndexFile(string indexPath)
    {
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

            // Calculate and store term frequencies for the document
            var tokenGroups = tokens.GroupBy(token => token);
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
                if (tdf.Count == 0)
                    emptyTerms.Add(term);
            }

            // Prune empty terms
            foreach (string term in emptyTerms)
            {
                _terms.Remove(term);
            }

            Console.WriteLine($"Removed from index: {path}");
        }
    }

    private void Save()
    {
        string jsonString = JsonSerializer.Serialize(new { _terms, _files });
        File.WriteAllText(_indexPath, jsonString);
    }

    /* Public Interface */

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
        Save();
    }

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
        Save();
    }

    public IndexInfo GetInfo()
    {
        return new IndexInfo(DocumentCount: _files.Count);
    }

    public IEnumerable<DocumentScore> CalculateDocumentScores(string query)
    {
        var scores = new Dictionary<Document, double>();

        foreach (string token in Tokenizer.ExtractTokens(query))
        {
            if (_terms.ContainsKey(token))
            {
                // Increment the score for each document that has the term
                foreach ((string docId, _) in _terms[token])
                {
                    Document doc = _files[docId];
                    if (!scores.ContainsKey(doc))
                        scores[doc] = 0;

                    scores[doc] = scores[doc] + Model!.CalculateScore(token, _files[docId]);
                }
            }
        }

        // Length-normalize using document length
        foreach ((Document doc, double score) in scores)
        {
            string docId = _fileIds[doc.Path];
            scores[doc] = scores[doc] / _files[docId].Length;
        }

        var result = scores
        .OrderByDescending((kvp) => kvp.Value)
        .Select((kvp) => new DocumentScore(Document: kvp.Key, Score: kvp.Value));

        return result;
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Returns the number of times the term occurs in the document
    /// </summary>
    public int GetTermFrequency(string term, Document d)
    {
        if (_terms.ContainsKey(term) && _fileIds.ContainsKey(d.Path))
        {
            return _terms[term][_fileIds[d.Path]];
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    ///     Returns the number of documents the term occurs in
    /// </summary>
    public int GetDocumentFrequency(string term)
    {
        if (_terms.ContainsKey(term))
        {
            return _terms[term].Count;
        }
        else
        {
            return 0;
        }
    }
}
