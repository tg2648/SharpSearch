using SharpSearch.Utilities;

namespace SharpSearch.UnitTests.Utilities;

[TestFixture]
public class TokenizerTests
{
    private Tokenizer _tokenizer = null!;

    [SetUp]
    public void SetUp()
    {
        _tokenizer = new Tokenizer();
    }

    [Test]
    public void ExtractTokens_Simple_ReturnsTokens()
    {
        var text = "one two three";
        string[] expected = { "one", "two", "three" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }
}