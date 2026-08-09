[![MIT](https://img.shields.io/github/license/Backs/Collections.Analyzer)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/Collections.Analyzer)](https://www.nuget.org/packages/Collections.Analyzer/)

Collections.Analyzer is a set of roslyn-based diagnostics for C#-projects that detect potential problems with operating
different collections.

# Motivation

For more information, see the following articles:

- [Harmful collection transformations](https://blog.rogatnev.net/posts/2022/01/Harmful-collection-transformations-part-3-collections.html)
- [Hunting the N+1 Query Problem with a Roslyn Analyzer](https://blog.rogatnev.net/posts/en/2026/08/N-Plus-One-Problem.html)

# Features

### Compiler warnings

Analyze your C#-code and warn about:
- **Redundant method calls** and inefficient collection transformations.
- **N+1 Query Problems** in loops and LINQ expressions when accessing repositories or services.

![Code fix string](https://raw.githubusercontent.com/Backs/Collections.Analyzer/master/Documentation/img/string-example-2.png)

### Code fixes

Automatically fixes found problems.

![Code fix enumerable](https://raw.githubusercontent.com/Backs/Collections.Analyzer/master/Documentation/img/enumerable-example-1.gif)

# Diagnostics

[Full set of diagnostics](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/Diagnostics.md)

# Installation

Every analyzer can be installed as a usual nuget-package. Just add a package reference to a project:

```
<PackageReference Include="Collections.Analyzer" Version="0.3.1" />
```

The analyzer will work only in the project it was added to. If you want to analyse all projects in your solution, you
can add file `Directory.build.props` to the solution directory with content:

```
<Project>
  <ItemGroup>
    <PackageReference Include="Collections.Analyzer" Version="0.3.1" />
  </ItemGroup>
</Project>
```

MSBuild will read these properties.