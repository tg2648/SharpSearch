using System.CommandLine;
using Moq;
using SharpSearch.Commands;
using SharpSearch.Indices;

namespace SharpSearch.Tests.Commands;

[TestFixture]
public class CommandRepositoryTests
{
    [Test]
    public void CommandRepository_ContainsAllCommands()
    {
        var mockIndex = new Mock<IIndex>(); ;
        var cr = new CommandRepository(mockIndex.Object);

        CollectionAssert.AllItemsAreInstancesOfType(cr.Commands, typeof(Command));
        CollectionAssert.AllItemsAreNotNull(cr.Commands);
        CollectionAssert.AllItemsAreUnique(cr.Commands);
        Assert.That(cr.Commands, Has.Exactly(4).Items);
    }
}