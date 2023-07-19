using SharpSearch.Models;
using SharpSearch.Utilities;
namespace SharpSearch.Tests.Utilities;

[TestFixture]
public class QueryResultPrinterTests
{
    StringWriter? stringWriter;
    List<DocumentScore>? testData;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        testData = new List<DocumentScore>()
        {
            new DocumentScore(new Document("a", 1, DateTime.Now), 0.01123),
            new DocumentScore(new Document("b", 1, DateTime.Now), 0.02123),
            new DocumentScore(new Document("c", 1, DateTime.Now), 0.03123),
            new DocumentScore(new Document("d", 1, DateTime.Now), 0.04123),
            new DocumentScore(new Document("e", 1, DateTime.Now), 0.05123),
            new DocumentScore(new Document("f", 1, DateTime.Now), 0.06123),
            new DocumentScore(new Document("g", 1, DateTime.Now), 0.07123),
            new DocumentScore(new Document("h", 1, DateTime.Now), 0.08123),
            new DocumentScore(new Document("i", 1, DateTime.Now), 0.09123),
            new DocumentScore(new Document("j", 1, DateTime.Now), 0.10123),
            new DocumentScore(new Document("k", 1, DateTime.Now), 0.11123),
        };
    }

    [SetUp]
    public void SetUp()
    {
        stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
    }

    [Test]
    public void PrintResults_SpecifyN_ReturnsNResults()
    {
        int n = 3;
        var expected = new string[] {
            "Query results for \"This is a query\":",
            "1. [0.01] a",
            "2. [0.02] b",
            "3. [0.03] c",
            string.Empty,
        };

        QueryResultPrinter.PrintResults("This is a query", testData!.Take(n), n);
        var result = stringWriter!.ToString().Split(Environment.NewLine);

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void PrintResults_EmptyResults_DoesNotReturnList()
    {
        int n = 3;
        var expected = new string[] {
            "Query results for \"This is a query\":",
            string.Empty,
        };

        QueryResultPrinter.PrintResults("This is a query", testData!.Take(0), n);
        var result = stringWriter!.ToString().Split(Environment.NewLine);

        CollectionAssert.AreEqual(expected, result);
    }
}