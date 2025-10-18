# CrossNGram Segmenter (CNSeg)

最小可运行版（MVP）按照《跨平台 n-gram 分词工具 — CNSeg SRS v1.0》完成项目结构梳理，新增 CLI、核心库与测试工程，便于后续扩展到 .NET MAUI 图形界面。

## 解决方案结构

```
CrossNGram.sln
├─ data/
│  └─ sample.txt
├─ src/
│  ├─ CrossNGram.CLI/        # CLI 入口与参数解析
│  └─ CrossNGram.Core/       # n-gram 模型与分词逻辑
└─ tests/
   └─ CrossNGram.Tests/      # 核心逻辑单元测试
```

现有 MAUI 演示项目仍保留在 `MAUIApp/`，后续 GUI 版本可复用 `CrossNGram.Core`。

## 快速开始

```bash
dotnet build CrossNGram.sln
dotnet run --project src/CrossNGram.CLI -- --input data/sample.txt --n 2 --threshold 1
```

如未提供 `--input` 与 `--output` 参数，则默认从标准输入读取、向标准输出写入。

## 发布单文件 CLI（win-x64）

```bash
dotnet publish src/CrossNGram.CLI/CrossNGram.CLI.csproj `
  -c Release `
  -r win-x64 `
  -p:PublishSingleFile=true `
  -p:SelfContained=true
```

构建完成后，最终发布物位于 `src/CrossNGram.CLI/bin/Release/net9.0/win-x64/publish/CrossNGram.CLI.exe`。

## 运行测试

```bash
dotnet test CrossNGram.sln
```

## 后续规划

- v1.1：引入配置文件（JSON/YAML），保持 CLI/核心分层。
- v2.0：创建 .NET MAUI GUI 项目，直接复用核心库与 CLI 配置解析思路。
