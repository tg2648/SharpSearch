using System.CommandLine;
using Moq;
using SharpSearch.Indices;

[TestFixture]
public class BaseCommandTests
{
    public RootCommand rootCommand;
    public Mock<IIndex> mockIndex;

    [SetUp]
    public void BaseSetUp()
    {
        rootCommand = new RootCommand();
        mockIndex = new Mock<IIndex>();
    }
}