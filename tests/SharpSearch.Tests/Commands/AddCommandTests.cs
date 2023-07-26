using System.CommandLine;
using Moq;
using SharpSearch.Commands;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class AddCommandTests : BaseCommandTests
{
    [SetUp]
    public void SetUp()
    {
        BaseSetUp();
        rootCommand!.AddCommand(new AddCommand(mockIndex!.Object).Command);
    }

    [Test]
    public void AddCommand_SinglePath_AddInvokedOnce()
    {
        rootCommand!.Invoke("add somepath");
        mockIndex!.Verify(index => index.Add(It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void AddCommand_TwoPaths_AddInvokedTwice()
    {
        rootCommand!.Invoke("add somepath1 somepath2");
        mockIndex!.Verify(index => index.Add(It.IsAny<string>()), Times.Exactly(2));
    }
}