using System.Text;

namespace CrossNGram.Core;

public static class TextUtils
{
    /// <summary>
    /// Normalizes whitespace and line endings to prepare text for tokenization.
    /// </summary>
    public static string NormalizeInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(input.Length);
        var lastWasSpace = false;

        foreach (var ch in input.Normalize(NormalizationForm.FormKC))
        {
            if (char.IsWhiteSpace(ch))
            {
                if (lastWasSpace)
                {
                    continue;
                }

                sb.Append(' ');
                lastWasSpace = true;
                continue;
            }

            sb.Append(ch);
            lastWasSpace = false;
        }

        return sb.ToString().Trim();
    }
}
