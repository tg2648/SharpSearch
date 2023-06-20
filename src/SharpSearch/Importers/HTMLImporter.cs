using SharpSearch.Utilities;

namespace SharpSearch.Importers;

public class HTMLImporter : IFileImporter
{
    /// <summary>
    /// Removes HTML tags (anything between `<` and `>`) from a line of text
    /// </summary>
    /// <returns></returns>
    private string RemoveTags(string line)
    {
        var sr = new StringReader(line);
        var sw = new StringWriter();

        bool insideTag = false;
        while (true)
        {
            int c = sr.Read();
            if (c == -1)
            {
                break;
            }

            char convertedCharacter = (char)c;
            if (convertedCharacter == '<')
            {
                insideTag = true;
            }
            else if (convertedCharacter == '>')
            {
                insideTag = false;
            }
            else if (!insideTag)
            {
                sw.Write(convertedCharacter);
            }
        }

        return sw.ToString();
    }

    /// <summary>
    /// Parses the given file into a collection of tokens.
    /// </summary>
    public IEnumerable<string> ExtractTokens(FileInfo file)
    {
        var tokenizer = new Tokenizer();
        using StreamReader sr = file.OpenText();

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            string withoutTags = RemoveTags(line);
            foreach (string token in tokenizer.ExtractTokens(withoutTags))
            {
                yield return token;
            }
        }
    }
}