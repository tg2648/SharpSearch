namespace SharpSearch.Importers;

interface IFileImporter
{
    IEnumerable<String> ExtractTokens(FileInfo file);
}
