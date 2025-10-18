using CrossNGram.Core;

namespace CrossNGram.Tests;

public sealed class TokenizerTests
{
    private const string ChineseText = "\u6211\u7231\u81ea\u7136\u8BED\u8A00\u5904\u7406";

    [Fact]
    public void Tokenize_SplitsChineseCharacters_WhenNGramsAreRare()
    {
        var tokenizer = new Tokenizer();

        var tokens = tokenizer.Tokenize(ChineseText, n: 2, threshold: 1);

        Assert.Equal(new[] { "\u6211", "\u7231", "\u81ea", "\u7136", "\u8BED", "\u8A00", "\u5904", "\u7406" }, tokens);
    }

    [Fact]
    public void Tokenize_IgnoresWhitespaceSegments()
    {
        var tokenizer = new Tokenizer();
        const string text = "MAUI \u8DE8\u5E73\u53F0";

        var tokens = tokenizer.Tokenize(text, n: 2, threshold: 1);

        Assert.Equal(new[] { "M", "A", "U", "I", "\u8DE8", "\u5E73", "\u53F0" }, tokens);
        Assert.DoesNotContain(tokens, string.IsNullOrWhiteSpace);
    }

    [Fact]
    public void Tokenize_ReturnsWholeString_WhenEveryGramIsFrequent()
    {
        var tokenizer = new Tokenizer();

        var tokens = tokenizer.Tokenize("\u4EBA\u4EBA\u4EBA\u4EBA", n: 2, threshold: 1);

        var single = Assert.Single(tokens);
        Assert.Equal("\u4EBA\u4EBA\u4EBA\u4EBA", single);
    }

    [Theory]
    [InlineData("", 2, 1)]
    [InlineData("   ", 2, 1)]
    public void Tokenize_ReturnsEmpty_ForBlankInput(string input, int n, int threshold)
    {
        var tokenizer = new Tokenizer();

        Assert.Empty(tokenizer.Tokenize(input, n, threshold));
    }
}
