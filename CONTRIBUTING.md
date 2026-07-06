# Contributing to HelloAssoDotnet

Thanks for your interest in contributing! This document explains how to build,
test, and propose changes.

## Prerequisites
- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or later.

## Build and test
```bash
# Restore and build the whole solution
dotnet build HelloAssoDotnet.sln -c Release

# Run the test suite
dotnet test HelloAssoDotnetTest/HelloAssoDotnetTest.csproj -c Release
```

## Running the sample
See [`samples/HelloAssoDotnet.Sample.Console`](samples/HelloAssoDotnet.Sample.Console/README.md).
Never commit real credentials: provide them via environment variables or a
local secrets file (both are git-ignored patterns).

## Making changes
1. Fork the repository and create a topic branch off `main`.
2. Keep changes focused and add or update tests when you change behavior.
3. Ensure `dotnet build` and `dotnet test` pass locally.
4. Do not commit secrets, credentials, or real personal/organization data. Test
   fixtures under `HelloAssoDotnetTest/Static/` must use anonymized placeholder
   values.
5. Open a pull request against `main` and fill in the PR template.

## Coding conventions
- Target framework, language version, and nullable/implicit-usings settings are
  centralized in `Directory.Build.props`.
- NuGet package versions are centrally managed in `Directory.Packages.props`
  (Central Package Management). Add new versions there rather than in individual
  `.csproj` files.
- Public APIs should keep XML documentation comments up to date.

## Reporting bugs and requesting features
Please use the issue templates under
[`.github/ISSUE_TEMPLATE`](.github/ISSUE_TEMPLATE).
