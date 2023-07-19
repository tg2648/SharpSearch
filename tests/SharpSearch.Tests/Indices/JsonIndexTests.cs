using SharpSearch.Indices;

namespace SharpSearch.Tests.Indices;

[TestFixture]
public class JsonIndexTests
{
    private string _indexPath;

    [SetUp]
    public void SetUp()
    {
        _indexPath = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(_indexPath);
    }

    [Test]
    public void JsonIndex_IndexInitiallyDoesNotExist_InitializedEmpty()
    {
        File.Delete(_indexPath);
        var index = new JsonIndex(_indexPath);
        Assert.Multiple(() =>
        {
            Assert.That(_indexPath, Does.Exist);
            Assert.That(File.ReadAllText(_indexPath), Is.Empty);
        });
    }

    [Test]
    public void JsonIndex_IndexAlreadyExists_ParsedCorrectly()
    {
        string json = @"{
            ""_terms"": {
                ""document"": {
                    ""abc"": 1
                }
            },
            ""_files"": {
                ""abc"": {
                    ""Path"": ""/some/path/doc.txt"",
                    ""Length"": 1,
                    ""ModifiedDate"": ""2023-06-28T17:42:30.3791176Z""
                }
            }
        }";
        File.WriteAllText(_indexPath, json);

        var index = new JsonIndex(_indexPath);
        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
    }

    [Test]
    public void JsonIndex_BadIndexAlreadyExists_Throws()
    {
        string json = @"{
            ""aaa"": {
                ""document"": {
                    ""abc"": 1
                }
            },
        }";
        File.WriteAllText(_indexPath, json);

        Assert.That(() => new JsonIndex(_indexPath), Throws.TypeOf<ApplicationException>());
    }

    [Test]
    public void GetDocumentFrequency_TermExists_ReturnsFrequency()
    {
        string json = @"{
            ""_terms"": {
                ""document"": {
                    ""abc"": 1
                }
            },
            ""_files"": {
                ""abc"": {
                    ""Path"": ""/some/path/doc.txt"",
                    ""Length"": 1,
                    ""ModifiedDate"": ""2023-06-28T17:42:30.3791176Z""
                }
            }
        }";
        File.WriteAllText(_indexPath, json);

        var index = new JsonIndex(_indexPath);
        Assert.That(index.GetDocumentFrequency("document"), Is.EqualTo(1));
    }
}