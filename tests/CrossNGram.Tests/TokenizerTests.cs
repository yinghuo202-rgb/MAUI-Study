using CrossNGram.Core;

namespace CrossNGram.Tests;

public sealed class TokenizerTests
{
    [Fact]
    public void Tokenize_ReturnsTokens_WhenThresholdSplits()
    {
        // Arrange
        var tokenizer = new Tokenizer();
        const string text = "我爱自然语言处理";

        // Act
        var tokens = tokenizer.Tokenize(text, n: 2, threshold: 1);

        // Assert
        Assert.Equal(new[] { "我", "爱", "自", "然", "语", "言", "处", "理" }, tokens);
    }

    [Theory]
    [InlineData("", 2, 1)]
    [InlineData("   ", 2, 1)]
    public void Tokenize_ReturnsEmpty_ForBlankInput(string input, int n, int threshold)
    {
        var tokenizer = new Tokenizer();

        Assert.Empty(tokenizer.Tokenize(input, n, threshold));
    }

    [Fact]
    public void Tokenize_ReturnsWholeString_WhenShorterThanN()
    {
        var tokenizer = new Tokenizer();

        var tokens = tokenizer.Tokenize("我爱", n: 3, threshold: 1);

        var token = Assert.Single(tokens);
        Assert.Equal("我爱", token);
    }
}
