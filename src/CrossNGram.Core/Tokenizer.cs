namespace CrossNGram.Core;

public sealed class Tokenizer
{
    private const int MinN = 2;
    private const int MinThreshold = 1;

    public IReadOnlyList<string> Tokenize(string text, int n = MinN, int threshold = MinThreshold)
    {
        if (n < MinN)
        {
            throw new ArgumentOutOfRangeException(nameof(n), $"n must be at least {MinN}.");
        }

        if (threshold < MinThreshold)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), $"threshold must be at least {MinThreshold}.");
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
                AddToken(tokens, normalized[start..boundary]);
                start = boundary;
            }
        }

        if (start < normalized.Length)
        {
            AddToken(tokens, normalized[start..]);
        }

        return tokens.ToArray();
    }

    private static void AddToken(ICollection<string> tokens, string candidate)
    {
        if (!string.IsNullOrWhiteSpace(candidate))
        {
            tokens.Add(candidate);
        }
    }
}
