using System.Collections.Immutable;
using System.Text;

namespace CrossNGram.Core;

/// <summary>
/// Provides n-gram based segmentation for UTF-8 text.
/// </summary>
public sealed class Tokenizer
{
    public IReadOnlyList<string> Tokenize(string text, int n = 2, int threshold = 1)
    {
        if (n <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be greater than zero.");
        }

        if (threshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "threshold must be non-negative.");
        }

        var normalized = TextUtils.NormalizeInput(text);
        if (string.IsNullOrEmpty(normalized))
        {
            return Array.Empty<string>();
        }

        if (normalized.Length <= n)
        {
            return new[] { normalized };
        }

        var model = NGramModel.FromText(normalized, n);
        var tokens = new List<string>();
        var start = 0;

        for (var boundary = 1; boundary < normalized.Length; boundary++)
        {
            var gramStart = boundary - 1;
            if (gramStart + n > normalized.Length)
            {
                continue;
            }

            var gram = normalized.AsSpan(gramStart, n);
            if (model.GetFrequency(gram) <= threshold)
            {
                tokens.Add(normalized[start..boundary]);
                start = boundary;
            }
        }

        if (start < normalized.Length)
        {
            tokens.Add(normalized[start..]);
        }

        return tokens.ToImmutableArray();
    }
}
