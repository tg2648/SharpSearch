interface IFileImporter
{
    IEnumerable<String> extractLines(FileInfo file);
}
