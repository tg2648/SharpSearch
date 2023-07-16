using SharpSearch.Importers;

namespace SharpSearch.UnitTests.Importers;

[TestFixture]
public class HTMLImporterTests
{
    FileInfo? file;

    [SetUp]
    public void SetUp()
    {
        file = new FileInfo("temp");
    }

    [TearDown]
    public void TearDown()
    {
        file!.Delete();
    }

    [Test]
    public void ExtractTokens_FileHasText_ReturnsTokens()
    {
        using (StreamWriter sw = file!.AppendText())
        {
            sw.WriteLine("<p>Hello <span>one</span></p>");
            sw.WriteLine("<b>two three</b>");
        }
        var expected = new string[] { "hello", "one", "two", "three" };

        var importer = new HTMLImporter();
        var result = importer.ExtractTokens(file).ToArray();

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void ExtractTokens_FileIsEmpty_ReturnsEmptyCollection()
    {
        using (StreamWriter sw = file!.AppendText())
        {
            sw.WriteLine("");
        }
        var expected = new List<string>();

        var importer = new TextImporter();
        var result = importer.ExtractTokens(file).ToList();

        CollectionAssert.AreEqual(expected, result);
    }
}
