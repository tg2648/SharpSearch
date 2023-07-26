using System.CommandLine;
using Moq;
using SharpSearch.Commands;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class QueryCommandTests : BaseCommandTests
{
    [SetUp]
    public void SetUp()
    {
        BaseSetUp();
        rootCommand!.AddCommand(new QueryCommand(mockIndex!.Object).Command);
    }

    [Test]
    public void QueryCommand_CalculateDocumentScoresInvokedOnce()
    {
        const string QUERY = "abcd";
        rootCommand!.Invoke($"query {QUERY}");
        mockIndex!.Verify(index => index.CalculateDocumentScores(QUERY), Times.Once());
    }
}