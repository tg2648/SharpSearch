using SharpSearch.Utilities;

namespace SharpSearch.UnitTests.Utilities;

[TestFixture]
public class TokenizerTests
{
    private Tokenizer _tokenizer = null!;

    [OneTimeSetUp]
    public void SetUp()
    {
        _tokenizer = new Tokenizer();
    }

    [Test]
    public void ExtractTokens_LettersOnly_ReturnsTokens()
    {
        var text = "one two three";
        string[] expected = { "one", "two", "three" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void ExtractTokens_DigitsOnly_ReturnsTokens()
    {
        var text = "1 23 456";
        string[] expected = { "456" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void ExtractTokens_LettersAndDigits_ReturnsTokens()
    {
        var text = "abc123 123abc";
        string[] expected = { "abc123", "123", "abc" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void ExtractTokens_Mixed_ReturnsTokens()
    {
        var text = "hello,world!123";
        string[] expected = { "hello", "world", "123" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void ExtractTokens_MixedCase_ReturnsLowercaseTokens()
    {
        var text = "ONE TwO threE four";
        string[] expected = { "one", "two", "three", "four" };
        var result = _tokenizer.ExtractTokens(text);

        CollectionAssert.AreEqual(expected, result);
    }
}