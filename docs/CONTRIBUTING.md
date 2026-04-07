# Contributing

Thank you for your interest in contributing to this project! We welcome contributions that help improve the demos, documentation, and tests.

## How to Contribute

1. **Fork** the repository and create a branch from `main`
2. **Make your changes** — see the guidelines below
3. **Test** your changes locally
4. **Submit a pull request** with a clear description of what you changed and why

## Contribution Guidelines

### Code

- Use **.NET 9** and C# 13 features where appropriate
- Follow the [.NET coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Add XML doc comments to public APIs
- Ensure all projects build without warnings

### Documentation

- Write in clear, concise English
- Use Markdown for all documentation files
- Include links to official Microsoft documentation where relevant
- Keep code samples short and focused

### Tests

- Add tests for any new functionality in the `tests/` folder
- Use [xUnit](https://xunit.net/) as the test framework (consistent with existing tests)
- Name tests clearly: `MethodName_Scenario_ExpectedResult`

## Folder Structure

```
src/             # Source code demos
docs/            # Documentation
scripts/         # Setup and utility scripts
tests/           # Unit and integration tests
```

## Reporting Issues

Please use GitHub Issues to report bugs or request new features. Include:
- A clear description of the problem or request
- Steps to reproduce (for bugs)
- Expected vs. actual behavior

## Code of Conduct

This project follows the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
