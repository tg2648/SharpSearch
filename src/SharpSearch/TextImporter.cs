
public class TextImporter : IFileImporter
{
    public IEnumerable<String> ExtractTokens(FileInfo file)
    {
        var tokenizer = new Tokenizer();
        using (StreamReader sr = file.OpenText())
        {
            String? line;
            while ((line = sr.ReadLine()) != null)
            {
                foreach (String token in tokenizer.ExtractTokens(line))
                {
                    yield return token;
                }
            }
        }
    }
}