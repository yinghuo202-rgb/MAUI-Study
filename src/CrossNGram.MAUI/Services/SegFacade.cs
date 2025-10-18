using CrossNGram.Core;
using System.Collections.Generic;

namespace CrossNGram.MAUI.Services;
public static class SegFacade
{
    public static IReadOnlyList<string> Segment(string text, int n = 2, int threshold = 1)
    {
        var tokenizer = new Tokenizer();
        return tokenizer.Tokenize(text, n, threshold);
    }
}
