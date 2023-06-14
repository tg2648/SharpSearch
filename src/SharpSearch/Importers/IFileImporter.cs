namespace SharpSearch.Importers;

interface IFileImporter
{
    IEnumerable<string> ExtractTokens(FileInfo file);
}
