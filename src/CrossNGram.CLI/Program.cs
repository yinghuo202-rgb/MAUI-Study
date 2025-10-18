using System.Text;
using CrossNGram.Core;

var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = utf8NoBom;

if (!CliOptions.TryParse(args, out var options, out var parseError))
{
    Console.Error.WriteLine($"Parameter error: {parseError}");
    Console.Error.WriteLine("Use --help to see command examples.");
    Environment.ExitCode = 1;
    return;
}

if (options.ShowHelp)
{
    PrintHelp();
    return;
}

try
{
    var text = await ReadInputAsync(options);
    var tokenizer = new Tokenizer();
    var tokens = tokenizer.Tokenize(text, options.N, options.Threshold);
    var output = string.Join(' ', tokens);

    await WriteOutputAsync(options, output);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Execution failed: {ex.Message}");
    Environment.ExitCode = 1;
}

static async Task<string> ReadInputAsync(CliOptions options)
{
    if (!string.IsNullOrWhiteSpace(options.InputPath))
    {
        if (!File.Exists(options.InputPath))
        {
            throw new FileNotFoundException("Input file not found.", options.InputPath);
        }

        return await File.ReadAllTextAsync(options.InputPath, Encoding.UTF8);
    }

    using var reader = new StreamReader(
        Console.OpenStandardInput(),
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        detectEncodingFromByteOrderMarks: true,
        leaveOpen: false);

    return await reader.ReadToEndAsync();
}

static async Task WriteOutputAsync(CliOptions options, string output)
{
    if (!string.IsNullOrWhiteSpace(options.OutputPath))
    {
        var directory = Path.GetDirectoryName(options.OutputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        await File.WriteAllTextAsync(options.OutputPath, output, utf8NoBom);
        return;
    }

    Console.WriteLine(output);
}

static void PrintHelp()
{
    const string help = """
        CrossNGram CLI - minimal n-gram based tokenizer

        Basic usage (stdin/stdout):
          echo "我爱自然语言处理" | dotnet run --project src/CrossNGram.CLI -- --n 2 --threshold 1

        File usage (UTF-8):
          dotnet run --project src/CrossNGram.CLI -- --input data/sample.txt --output data/result.txt --n 3 --threshold 2

        Options:
          --input <path> / -i <path>       Source file (defaults to STDIN)
          --output <path> / -o <path>      Target file (defaults to STDOUT)
          --n <value> / -n <value>         n-gram window size, range [2, 9], default 2
          --threshold <value> / -t <value> Frequency threshold, range [1, 99], default 1
          --help / -h                      Print help text
        """;

    Console.WriteLine(help);
}

internal sealed record CliOptions(
    string? InputPath,
    string? OutputPath,
    int N,
    int Threshold,
    bool ShowHelp)
{
    private const int DefaultN = 2;
    private const int DefaultThreshold = 1;
    private const int MinN = 2;
    private const int MaxN = 9;
    private const int MinThreshold = 1;
    private const int MaxThreshold = 99;

    public static bool TryParse(string[] args, out CliOptions options, out string error)
    {
        string? input = null;
        string? output = null;
        var n = DefaultN;
        var threshold = DefaultThreshold;
        var showHelp = false;

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];
            switch (current)
            {
                case "--help":
                case "-h":
                    showHelp = true;
                    continue;
                case "--input":
                case "-i":
                    if (!TryReadValue(args, ref i, current, out var inputValue, out error))
                    {
                        options = Default();
                        return false;
                    }

                    input = inputValue;
                    break;
                case "--output":
                case "-o":
                    if (!TryReadValue(args, ref i, current, out var outputValue, out error))
                    {
                        options = Default();
                        return false;
                    }

                    output = outputValue;
                    break;
                case "--n":
                case "-n":
                    if (!TryReadValue(args, ref i, current, out var nRaw, out error) ||
                        !TryParseInt(nRaw, MinN, MaxN, "n", out n, out error))
                    {
                        options = Default();
                        return false;
                    }
                    break;
                case "--threshold":
                case "-t":
                    if (!TryReadValue(args, ref i, current, out var thresholdRaw, out error) ||
                        !TryParseInt(thresholdRaw, MinThreshold, MaxThreshold, "threshold", out threshold, out error))
                    {
                        options = Default();
                        return false;
                    }
                    break;
                default:
                    options = Default();
                    error = $"Unknown option: {current}";
                    return false;
            }
        }

        options = new CliOptions(input, output, n, threshold, showHelp);
        error = string.Empty;
        return true;
    }

    private static CliOptions Default() => new(null, null, DefaultN, DefaultThreshold, false);

    private static bool TryReadValue(
        IReadOnlyList<string> args,
        ref int index,
        string option,
        out string value,
        out string error)
    {
        if (index + 1 >= args.Count)
        {
            value = string.Empty;
            error = $"Missing value for {option}.";
            return false;
        }

        index += 1;
        value = args[index];
        error = string.Empty;
        return true;
    }

    private static bool TryParseInt(
        string text,
        int min,
        int max,
        string name,
        out int result,
        out string error)
    {
        if (!int.TryParse(text, out result))
        {
            error = $"{name} expects an integer but received '{text}'.";
            return false;
        }

        if (result < min || result > max)
        {
            error = $"{name} must be within [{min}, {max}].";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
