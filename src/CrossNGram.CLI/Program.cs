
using System.Text;
using CrossNGram.Core;

// 强制使用 UTF-8（无 BOM）
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);




Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var options = CliOptions.Parse(args);
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
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.ExitCode = 1;
}

static async Task<string> ReadInputAsync(CliOptions options)
{
    if (!string.IsNullOrWhiteSpace(options.InputPath))
    {
        return await File.ReadAllTextAsync(options.InputPath!, Encoding.UTF8);
    }

    using var reader = new StreamReader(Console.OpenStandardInput(), Encoding.UTF8);
    return await reader.ReadToEndAsync();
}

static async Task WriteOutputAsync(CliOptions options, string output)
{
    if (!string.IsNullOrWhiteSpace(options.OutputPath))
    {
        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        await File.WriteAllTextAsync(options.OutputPath!, output, utf8NoBom);
        return;
    }

    Console.WriteLine(output);
}

static void PrintHelp()
{
    var help = """
        CrossNGram Segmenter (CNSeg) CLI

        Usage:
          dotnet run --project src/CrossNGram.CLI -- [options]

        Options:
          --input <path>       Optional UTF-8 text file to read.
          --output <path>      Optional output file for segmented result.
          --n <value>          N-gram length (default: 2).
          --threshold <value>  Cut-off frequency to split tokens (default: 1).
          --help               Display this help message.

        Reads from STDIN and writes to STDOUT when paths are omitted.
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
    public static CliOptions Parse(string[] args)
    {
        string? input = null;
        string? output = null;
        var n = 2;
        var threshold = 1;
        var showHelp = false;

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];
            switch (current)
            {
                case "--help":
                case "-h":
                    showHelp = true;
                    break;
                case "--input":
                    input = ReadValue(args, ref i, "--input");
                    break;
                case "--output":
                    output = ReadValue(args, ref i, "--output");
                    break;
                case "--n":
                    n = ParseInt(ReadValue(args, ref i, "--n"), 1, nameof(n));
                    break;
                case "--threshold":
                    threshold = ParseInt(ReadValue(args, ref i, "--threshold"), 0, nameof(threshold));
                    break;
                default:
                    throw new ArgumentException($"Unrecognized option '{current}'. Use --help for usage.");
            }
        }

        return new CliOptions(input, output, n, threshold, showHelp);
    }

    private static string ReadValue(IReadOnlyList<string> args, ref int index, string option)
    {
        if (index + 1 >= args.Count)
        {
            throw new ArgumentException($"Missing value for {option}.");
        }

        index += 1;
        return args[index];
    }

    private static int ParseInt(string value, int minValue, string name)
    {
        if (!int.TryParse(value, out var result))
        {
            throw new ArgumentException($"Value '{value}' for {name} is not a valid integer.");
        }

        if (result < minValue)
        {
            throw new ArgumentOutOfRangeException(name, $"Value must be greater than or equal to {minValue}.");
        }

        return result;
    }
}
