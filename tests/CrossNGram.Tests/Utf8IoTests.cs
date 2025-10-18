using System.Text;

namespace CrossNGram.Tests;

public sealed class Utf8IoTests
{
    [Fact]
    public void SampleFile_IsUtf8WithoutBom()
    {
        var samplePath = Path.Combine(GetRepositoryRoot(), "data", "sample.txt");
        Assert.True(File.Exists(samplePath));

        var bytes = File.ReadAllBytes(samplePath);
        Assert.NotEmpty(bytes);
        Assert.False(HasBom(bytes));

        var content = File.ReadAllText(samplePath, Encoding.UTF8);
        Assert.Contains("\u6211\u7231\u81EA\u7136\u8BED\u8A00", content);
    }

    [Fact]
    public async Task Utf8RoundTrip_ProducesNoBom()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"crossngram_{Guid.NewGuid():N}.txt");
        try
        {
            const string payload = "\u6DF7\u5408\u6587\u672C Mixed UTF-8";
            var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            await File.WriteAllTextAsync(tempPath, payload, utf8NoBom);

            var bytes = await File.ReadAllBytesAsync(tempPath);
            Assert.False(HasBom(bytes));

            var roundTrip = await File.ReadAllTextAsync(tempPath, Encoding.UTF8);
            Assert.Equal(payload, roundTrip);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private static bool HasBom(IReadOnlyList<byte> bytes) =>
        bytes.Count >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

    private static string GetRepositoryRoot()
    {
        var assemblyDirectory = Path.GetDirectoryName(typeof(Utf8IoTests).Assembly.Location) ?? AppContext.BaseDirectory;
        return Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "..", "..", ".."));
    }
}
