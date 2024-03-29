namespace SharpSearch.Utilities;

public class Tokenizer
{
    private const int LENGTH_THRESHOLD = 2;

    private static int TrimLeft(string text, int curr)
    {
        while (curr < text.Length && char.IsWhiteSpace(text[curr]))
            curr++;

        return curr;
    }

    private static int ChopWhile(string text, int curr, Predicate<char> predicate)
    {
        while (curr < text.Length && predicate(text[curr]))
            curr++;

        return curr;
    }

    /// <summary>
    ///     Returns tokens from a given text <br/>
    /// </summary>
    /// <returns>
    ///     Collection of tokens
    /// </returns>
    /// <remarks>
    ///     123abc is tokenized as 123 and abc <br/>
    ///     abc123 is tokenized as abc123 <br/>
    ///     Non-letter or non-digit characters are tokenized as their own tokens,
    ///     which get filtered out due to length. <br/>
    /// </remarks>
    public static IEnumerable<string> ExtractTokens(string text)
    {
        // TODO: stopword elimination
        int curr = 0;
        int next = 0;

        string token;
        while (true)
        {
            curr = TrimLeft(text, curr);
            if (curr >= text.Length) break;

            if (char.IsLetter(text[curr]))
            {
                next = ChopWhile(text, curr, char.IsLetterOrDigit);
            }
            else if (char.IsDigit(text[curr]))
            {
                next = ChopWhile(text, curr, char.IsDigit);
            }
            else
            {
                next = curr + 1;
            }

            token = text[curr..next].ToLower();
            if (token.Length > LENGTH_THRESHOLD)
            {
                yield return token;
            }

            curr = next;
        }
    }
}
