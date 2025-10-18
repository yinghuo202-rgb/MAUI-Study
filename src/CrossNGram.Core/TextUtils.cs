using System.Text;

namespace CrossNGram.Core;

public static class TextUtils
{
    public static string NormalizeInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(input.Length);
        var lastWasSpace = false;

        foreach (var ch in input.Normalize(NormalizationForm.FormKC))
        {
            if (char.IsWhiteSpace(ch))
            {
                if (lastWasSpace)
                {
                    continue;
                }

                builder.Append(' ');
                lastWasSpace = true;
            }
            else
            {
                builder.Append(ch);
                lastWasSpace = false;
            }
        }

        return builder.ToString().Trim();
    }
}
