using SharpSearch.Utilities;

namespace SharpSearch.Importers;

public class TextImporter : IFileImporter
{
    /// <summary>
    /// Parses the given file into a collection of tokens.
    /// </summary>
    public IEnumerable<string> ExtractTokens(FileInfo file)
    {
        using StreamReader sr = file.OpenText();

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            foreach (string token in Tokenizer.ExtractTokens(line))
            {
                yield return token;
            }
        }
    }
}