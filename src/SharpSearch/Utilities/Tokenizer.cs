namespace SharpSearch.Utilities;

public class Tokenizer
{
    public IEnumerable<String> ExtractTokens(String text)
    {
        // TODO: stopword elimination
        // TODO: length filtering (>2)
        // TODO: punctuation trimming
        var tokens = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return tokens;
    }
}
