using SharpSearch.Indices;

namespace SharpSearch.Tests.Indices;

[TestFixture]
public class JsonIndexTests
{
    private string? _indexPath;
    private List<string>? _tempFiles;
    private List<string>? _tempDirs;

    [SetUp]
    public void SetUp()
    {
        _indexPath = Path.GetTempFileName();
        _tempFiles = new();
        _tempDirs = new();
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(_indexPath!);
        foreach (var filePath in _tempFiles!)
        {
            File.Delete(filePath);
        }
        foreach (var dirPath in _tempDirs!)
        {
            Directory.Delete(dirPath);
        }
    }

    [Test]
    public void JsonIndex_IndexInitiallyDoesNotExist_InitializedEmpty()
    {
        File.Delete(_indexPath!);
        var index = new JsonIndex(_indexPath!);
        Assert.Multiple(() =>
        {
            Assert.That(_indexPath, Does.Exist);
            Assert.That(File.ReadAllText(_indexPath!), Is.Empty);
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
        File.WriteAllText(_indexPath!, json);

        var index = new JsonIndex(_indexPath!);
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
        File.WriteAllText(_indexPath!, json);

        Assert.That(() => new JsonIndex(_indexPath!), Throws.TypeOf<ApplicationException>());
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
        File.WriteAllText(_indexPath!, json);

        var index = new JsonIndex(_indexPath!);
        Assert.That(index.GetDocumentFrequency("document"), Is.EqualTo(1));
    }

    [Test]
    public void Add_PathDoesNotExist_Throws()
    {
        var index = new JsonIndex(_indexPath!);
        Assert.That(() => index.Add("abc"), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Add_FileExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath!);
        string tmpFilePath = Path.GetTempFileName();
        string txtFilePath = Path.ChangeExtension(tmpFilePath, ".txt");
        File.WriteAllText(txtFilePath, "Hello World!");
        _tempFiles!.Add(txtFilePath);
        _tempFiles!.Add(tmpFilePath);

        index.Add(txtFilePath);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(1));
    }

    [Test]
    public void Add_DirectoryExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath!);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        _tempDirs!.Add(tempDir.FullName);

        string tempPath1 = Path.Combine(tempDir.FullName, "a.txt");
        _tempFiles!.Add(tempPath1);
        File.WriteAllText(tempPath1, "Hello World!");

        string tempPath2 = Path.Combine(tempDir.FullName, "b.txt");
        _tempFiles!.Add(tempPath2);
        File.WriteAllText(tempPath2, "Hello World!");

        index.Add(tempDir.FullName);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(2));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(2));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(2));
    }

    [Test]
    public void Add_FileChangedAndReadded_IndexIsCorrect()
    {
        var index = new JsonIndex(_indexPath!);
        string tmpFilePath = Path.GetTempFileName();
        string txtFilePath = Path.ChangeExtension(tmpFilePath, ".txt");
        _tempFiles!.Add(tmpFilePath);
        _tempFiles!.Add(txtFilePath);

        File.WriteAllText(txtFilePath, "Hello World!");
        index.Add(txtFilePath);
        File.WriteAllText(txtFilePath, "One Two!");
        index.Add(txtFilePath);

        Assert.Multiple(() =>
        {
            Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
            Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(0));
            Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(0));
            Assert.That(index.GetDocumentFrequency("one"), Is.EqualTo(1));
            Assert.That(index.GetDocumentFrequency("two"), Is.EqualTo(1));
        });
    }

    [Test]
    public void Remove_DirectoryExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath!);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        _tempDirs!.Add(tempDir.FullName);

        string tempPath1 = Path.Combine(tempDir.FullName, "a.txt");
        _tempFiles!.Add(tempPath1);
        File.WriteAllText(tempPath1, "Hello World!");

        string tempPath2 = Path.Combine(tempDir.FullName, "b.txt");
        _tempFiles!.Add(tempPath2);
        File.WriteAllText(tempPath2, "Hello World!");

        index.Add(tempDir.FullName);
        index.Remove(tempDir.FullName);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(0));
    }

    [Test]
    public void Remove_FileExists_UpdatesIndex()
    {
        var index = new JsonIndex(_indexPath!);
        DirectoryInfo tempDir = Directory.CreateTempSubdirectory();
        _tempDirs!.Add(tempDir.FullName);

        string tempPath1 = Path.Combine(tempDir.FullName, "a.txt");
        _tempFiles!.Add(tempPath1);
        File.WriteAllText(tempPath1, "Hello World!");

        string tempPath2 = Path.Combine(tempDir.FullName, "b.txt");
        _tempFiles!.Add(tempPath2);
        File.WriteAllText(tempPath2, "Hello World!");

        index.Add(tempDir.FullName);
        index.Remove(tempPath1);

        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(1));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(1));
    }

    [Test]
    public void Remove_PathDoesNotExist_Throws()
    {
        var index = new JsonIndex(_indexPath!);
        Assert.That(() => index.Add("abc"), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Prune_RemovesFromIndex()
    {
        var index = new JsonIndex(_indexPath!);
        string tmpFilePath = Path.GetTempFileName();
        string txtFilePath = Path.ChangeExtension(tmpFilePath, ".txt");
        File.AppendAllText(txtFilePath, "Hello World!");
        index.Add(txtFilePath);
        File.Delete(tmpFilePath);
        File.Delete(txtFilePath);

        int pruned = index.Prune();

        Assert.That(pruned, Is.EqualTo(1));
        Assert.That(index.GetInfo().DocumentCount, Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("hello"), Is.EqualTo(0));
        Assert.That(index.GetDocumentFrequency("world"), Is.EqualTo(0));
    }
}