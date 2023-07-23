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

    [Test]
    public void Add_PathDoesNotExist_Throws()
    {
        var index = new JsonIndex(_indexPath);
        Assert.That(() => index.Add("abc"), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Add_FileExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath);
        string filePath = Path.GetTempFileName();
        filePath = Path.ChangeExtension(filePath, ".txt");
        File.AppendAllText(filePath, "Hello World!");
        index.Add(filePath);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(1));

        File.Delete(filePath);
    }

    [Test]
    public void Add_DirectoryExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        using (StreamWriter f1 = File.CreateText(Path.Combine(tempDir.FullName, "a.txt")))
        {
            f1.WriteLine("Hello World!");
        }
        using (StreamWriter f2 = File.CreateText(Path.Combine(tempDir.FullName, "b.txt")))
        {
            f2.WriteLine("Hello World!");
        }
        index.Add(tempDir.FullName);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(2));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(2));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(2));

        foreach (FileInfo file in tempDir.GetFiles())
        {
            file.Delete();
        }
        tempDir.Delete();
    }

    [Test]
    public void Remove_DirectoryExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        using (StreamWriter f1 = File.CreateText(Path.Combine(tempDir.FullName, "a.txt")))
        {
            f1.WriteLine("Hello World!");
        }
        using (StreamWriter f2 = File.CreateText(Path.Combine(tempDir.FullName, "b.txt")))
        {
            f2.WriteLine("Hello World!");
        }
        index.Add(tempDir.FullName);
        index.Remove(tempDir.FullName);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(0));

        foreach (FileInfo file in tempDir.GetFiles())
        {
            file.Delete();
        }
        tempDir.Delete();
    }

    [Test]
    public void Remove_FileExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        using (StreamWriter f1 = File.CreateText(Path.Combine(tempDir.FullName, "a.txt")))
        {
            f1.WriteLine("Hello World!");
        }
        using (StreamWriter f2 = File.CreateText(Path.Combine(tempDir.FullName, "b.txt")))
        {
            f2.WriteLine("Hello World!");
        }
        index.Add(tempDir.FullName);
        index.Remove(Path.Combine(tempDir.FullName, "a.txt"));

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(1));

        foreach (FileInfo file in tempDir.GetFiles())
        {
            file.Delete();
        }
        tempDir.Delete();
    }

    [Test]
    public void Remove_PathDoesNotExist_Throws()
    {
        var index = new JsonIndex(_indexPath);
        Assert.That(() => index.Add("abc"), Throws.TypeOf<ArgumentException>());
    }
}