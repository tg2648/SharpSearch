using System.CommandLine;
using Moq;
using SharpSearch.Commands;
using SharpSearch.Indices;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class PruneCommandTests : BaseCommandTests
{
    [SetUp]
    public void SetUp()
    {
        BaseSetUp();
        rootCommand!.AddCommand(new PruneCommand(mockIndex!.Object).Command);
        mockIndex.Setup(x => x.Prune()).Returns(1);
    }

    [Test]
    public void PruneCommand_PruneInvokedOnce()
    {
        rootCommand!.Invoke("prune");
        mockIndex!.Verify(index => index.Prune(), Times.Once());
    }
}