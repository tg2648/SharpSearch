using Moq;
using SharpSearch.Indices;
using SharpSearch.Models;

[TestFixture]
class TfIdfModelTests
{
    [Test]
    public void CalculateScore_TermIsInAllDocuments_ScoreIsZero()
    {
        var mockIndex = new Mock<IIndex>();
        var sampleDoc = new Document("path", 1, DateTime.Now);

        mockIndex.Setup(m => m.GetTermFrequency("hello", sampleDoc)).Returns(1);
        mockIndex.Setup(m => m.GetDocumentFrequency("hello")).Returns(5);
        mockIndex.Setup(m => m.GetInfo()).Returns(new IndexInfo(5));

        var model = new TfIdfModel();
        model.Index = mockIndex.Object;

        Assert.That(model.CalculateScore("hello", sampleDoc), Is.EqualTo(0));
    }

    [Test]
    public void CalculateScore_TermIsRare_ScoreIsCorrect()
    {
        var mockIndex = new Mock<IIndex>();
        var sampleDoc = new Document("path", 1, DateTime.Now);

        mockIndex.Setup(m => m.GetTermFrequency("hello", sampleDoc)).Returns(1);
        mockIndex.Setup(m => m.GetDocumentFrequency("hello")).Returns(1);
        mockIndex.Setup(m => m.GetInfo()).Returns(new IndexInfo(10));

        var model = new TfIdfModel();
        model.Index = mockIndex.Object;

        Assert.That(model.CalculateScore("hello", sampleDoc), Is.EqualTo(0.301).Within(0.005));
    }
}