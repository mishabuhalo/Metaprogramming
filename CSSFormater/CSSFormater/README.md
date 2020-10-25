# Pre instalation

Before run this formatting tool you should install dotnet core 3.1 on your machine

[download](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Usage

```bash
dotnet run [arguments]

Arguments:

-h, --help Show help message
-verify -(project|directory|file) "path"  Verify file project or directory and write errors to log file
```

### Usage Example

```bash
dotnet run -verify -file "TestFiles\2.css"
```
