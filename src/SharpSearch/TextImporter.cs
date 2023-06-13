
public class TextImporter : IFileImporter
{
    public IEnumerable<String> extractLines(FileInfo file)
    {
        using (StreamReader sr = file.OpenText())
        {
            String? line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
            yield break;
        }
    }
}