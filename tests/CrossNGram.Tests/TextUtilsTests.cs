using CrossNGram.Core;

namespace CrossNGram.Tests;

public sealed class TextUtilsTests
{
    [Fact]
    public void NormalizeInput_CompressesWhitespaceAndTrims()
    {
        const string input = "  MAUI\tis\r\na cross-platform framework ";

        var normalized = TextUtils.NormalizeInput(input);

        Assert.Equal("MAUI is a cross-platform framework", normalized);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeInput_ReturnsEmpty_ForNullOrWhitespace(string? input)
    {
        var normalized = TextUtils.NormalizeInput(input);

        Assert.Equal(string.Empty, normalized);
    }
}
