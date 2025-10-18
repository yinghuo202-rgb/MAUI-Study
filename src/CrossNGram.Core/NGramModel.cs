using System.Buffers;
using System.Collections.Immutable;

namespace CrossNGram.Core;

/// <summary>
/// Aggregates n-gram frequencies for a chunk of text.
/// </summary>
public sealed class NGramModel
{
    private readonly ImmutableDictionary<string, int> _frequencies;

    private NGramModel(ImmutableDictionary<string, int> frequencies)
    {
        _frequencies = frequencies;
    }

    public static NGramModel FromText(string text, int n)
    {
        if (n <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n must be greater than zero.");
        }

        if (string.IsNullOrEmpty(text) || text.Length < n)
        {
            return new NGramModel(ImmutableDictionary<string, int>.Empty);
        }

        var map = new Dictionary<string, int>(StringComparer.Ordinal);

        var buffer = ArrayPool<char>.Shared.Rent(n);
        try
        {
            for (var i = 0; i <= text.Length - n; i++)
            {
                text.CopyTo(i, buffer, 0, n);
                var gram = new string(buffer, 0, n);

                map.TryGetValue(gram, out var count);
                map[gram] = count + 1;
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        return new NGramModel(map.ToImmutableDictionary(StringComparer.Ordinal));
    }

    public int GetFrequency(ReadOnlySpan<char> gram)
    {
        if (_frequencies.Count == 0 || gram.IsEmpty)
        {
            return 0;
        }

        return _frequencies.TryGetValue(gram.ToString(), out var value) ? value : 0;
    }
}
