using System.CommandLine;
using Moq;
using SharpSearch.Commands;
using SharpSearch.Indices;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class InfoCommandTests : BaseCommandTests
{
    [SetUp]
    public void SetUp()
    {
        BaseSetUp();
        rootCommand!.AddCommand(new InfoCommand(mockIndex!.Object).Command);
        mockIndex.Setup(x => x.GetInfo()).Returns(new IndexInfo(default));
    }

    [Test]
    public void InfoCommand_GetInfoInvokedOnce()
    {
        rootCommand!.Invoke("info");
        mockIndex!.Verify(index => index.GetInfo(), Times.Once());
    }
}