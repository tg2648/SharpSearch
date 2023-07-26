using System.CommandLine;
using Moq;
using SharpSearch.Commands;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class RemoveCommandTests : BaseCommandTests
{
    [SetUp]
    public void SetUp()
    {
        BaseSetUp();
        rootCommand!.AddCommand(new RemoveCommand(mockIndex!.Object).Command);
    }

    [Test]
    public void RemoveCommand_SinglePath_RemoveInvokedOnce()
    {
        rootCommand!.Invoke("remove somepath");
        mockIndex!.Verify(index => index.Remove(It.IsAny<string>()), Times.Once());
    }

    [Test]
    public void RemoveCommand_TwoPaths_RemoveInvokedTwice()
    {
        rootCommand!.Invoke("remove somepath1 somepath2");
        mockIndex!.Verify(index => index.Remove(It.IsAny<string>()), Times.Exactly(2));
    }
}