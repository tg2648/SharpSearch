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
            new DocumentScore("a", 0.01123),
            new DocumentScore("b", 0.02123),
            new DocumentScore("c", 0.03123),
            new DocumentScore("d", 0.04123),
            new DocumentScore("e", 0.05123),
            new DocumentScore("f", 0.06123),
            new DocumentScore("g", 0.07123),
            new DocumentScore("h", 0.08123),
            new DocumentScore("i", 0.09123),
            new DocumentScore("j", 0.10123),
            new DocumentScore("k", 0.11123),
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