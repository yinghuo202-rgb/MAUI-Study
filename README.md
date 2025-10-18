# CrossNGram Segmenter

CrossNGram is a minimal n-gram based tokenizer that ships with:

- `CrossNGram.Core`: core text utilities and tokenization logic
- `CrossNGram.CLI`: command line interface with UTF-8 input/output
- `CrossNGram.MAUI`: WinUI 3 front-end with basic validation
- `CrossNGram.Tests`: xUnit coverage for normalization, tokenization, and UTF-8 IO

## Build and Test

```powershell
dotnet build .\CrossNGram.sln -c Debug
dotnet test  .\CrossNGram.sln
```

## CLI Usage

```powershell
# stdin to stdout (UTF-8)
echo "我爱自然语言处理" | dotnet run --project .\src\CrossNGram.CLI -- --n 2 --threshold 1

# file input/output (target directory is created when necessary)
dotnet run --project .\src\CrossNGram.CLI -- `
  --input .\data\sample.txt `
  --output .\data\result.txt `
  --n 3 `
  --threshold 2
```

- `--n` accepts values in `[2, 9]`
- `--threshold` accepts values in `[1, 99]`
- failures print to `stderr` and exit with a non-zero code

## MAUI (WinUI 3) App

```powershell
dotnet run --project .\src\CrossNGram.MAUI\CrossNGram.MAUI.csproj `
  -c Debug `
  -f net9.0-windows10.0.19041.0
```

- requires Windows App SDK Runtime 1.6 or newer
- parameters constrained to `n ∈ [2,5]`, `threshold ∈ [1,9]`
- input larger than 32KB (UTF-8) disables the tokenize button and prompts the user to switch to the CLI

## CLI Single-File Publish (win-x64)

```powershell
dotnet publish .\src\CrossNGram.CLI\CrossNGram.CLI.csproj `
  -c Release `
  -r win-x64 `
  -p:SelfContained=true `
  -p:PublishSingleFile=true `
  -p:PublishReadyToRun=true `
  -p:EnableCompressionInSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

The executable is produced at `src/CrossNGram.CLI/bin/Release/net9.0/win-x64/publish/CrossNGram.CLI.exe`.
