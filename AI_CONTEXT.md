# AI Context: Collections.Analyzer

This file is intended to help AI agents quickly understand the project structure and rules for making changes.

## Project Overview
`Collections.Analyzer` is a set of Roslyn analyzers (diagnostics) and code fixes for C#. The project focuses on optimizing collection usage, detecting redundant transformations (e.g., `ToArray()` when not needed), and suggesting more efficient alternatives.

## Technology Stack
- **Languages**: C#, Roslyn (Microsoft.CodeAnalysis).
- **Target Frameworks**:
  - `netstandard2.0`: Core logic for analyzers and fixes (for compatibility with Visual Studio and MSBuild).
  - `net8.0`, `net9.0`, `net10.0`: Tests and benchmarks.
- **Testing**: NUnit + Microsoft.CodeAnalysis.Testing.
- **Benchmarks**: BenchmarkDotNet.

## Solution Structure
- **`Collections.Analyzer/`**: Core diagnostics logic.
  - `Diagnostics/`: Each diagnostic has its own subfolder `CIxxxx/`.
  - `Resources.resx`: String resources for message localization (English and Russian).
- **`Collections.Analyzer.CodeFixes/`**: Implementation of code fixes.
  - Contains `CodeFixProvider` for corresponding diagnostics.
- **`Tests/`**: Unit tests.
  - `Resources/`: Contains source code files (`.cs`) used by tests to verify diagnostic triggering and fix correctness.
- **`Documentation/`**: Description of all diagnostics in Markdown format.
- **`Examples/`**: "Bad" vs "Good" code examples.
- **`Benchmarks/`**: Performance measurements to justify rules.

## Instructions for AI on Solving Tasks

### 1. Adding a New Diagnostic
- **Identifier**: The next available `CIxxxx` from `Documentation/Diagnostics.md`.
- **Analyzer Class**: Create in `Collections.Analyzer/Diagnostics/CIxxxx/`. Inherit from `DiagnosticAnalyzer`.
- **Localization**: Add title and description to `Resources.resx` and `Resources.ru.resx`.
- **Registration**: Ensure the analyzer is marked with the `[DiagnosticAnalyzer(LanguageNames.CSharp)]` attribute.

### 2. Adding a Code Fix
- Create or update a class in `Collections.Analyzer.CodeFixes`.
- Inherit from `CodeFixProvider`.
- Specify the new diagnostic ID in `FixableDiagnosticIds`.

### 3. Writing Tests
- Tests should be located in the `Tests` project; the folder should match the diagnostic name.
- Place source code for tests in `Tests/Resources/<DiagnosticName>/`.
- Use `ResourceReader.ReadFromFile()` to load the code.
- Use `{|CIxxxx:code|}` syntax in resource files to mark expected diagnostics.
- Verify both the presence of the diagnostic and the result of applying the fix (if applicable).

### 4. Documentation and Releases
- After creating a diagnostic, be sure to add a description in `Documentation/CIxxxx.md` (based on existing templates) and update `Documentation/Diagnostics.md`.
- Update `CHANGELOG.md`.
- If `AnalyzerReleases.Shipped.md` and `AnalyzerReleases.Unshipped.md` are used, update them (Roslyn analyzer release tracking).

## Codebase Specifics
- **File-scoped namespaces** are used.
- The project actively uses extensions for working with the syntax tree (`ExpressionExtensions.cs`, `StringExtensions.cs`).
- When analyzing, it is important to consider the semantic model (`context.SemanticModel`) to check data types, not just method names.
