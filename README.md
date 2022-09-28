[![MIT](https://img.shields.io/github/license/Backs/Collections.Analyzer)](LICENSE)
[![AppVeyor](https://img.shields.io/appveyor/build/Backs/CollectionsAnalyzer)](https://ci.appveyor.com/project/Backs/CollectionsAnalyzer)
[![Nuget](https://img.shields.io/nuget/v/Collections.Analyzer)](https://www.nuget.org/packages/Collections.Analyzer/)

Collections.Analyzer is a set of roslyn-based diagnostics for C#-projects that detect potential problems with operating
different collections.

# Motivation

For more information, see the following articles:

- [Part 1: string to array of chars](https://blog.rogatnev.net/posts/2021/09/Harmful-collection-transformations-part-1-strings.html)
- [Part 2: automatic diagnostics](https://blog.rogatnev.net/posts/2021/10/Harmful-collection-transformations-part-2-diagnostics.html)
- [Part 3: collections](https://blog.rogatnev.net/posts/2022/01/Harmful-collection-transformations-part-3-collections.html)

# Features

### Compiler warnings

Analyze your C#-code and warn about redundant method calls.

![Code fix string](https://raw.githubusercontent.com/Backs/Collections.Analyzer/master/Documentation/img/string-example-2.png)

### Code fixes

Automatically fixes found problems.

![Code fix enumerable](https://raw.githubusercontent.com/Backs/Collections.Analyzer/master/Documentation/img/enumerable-example-1.gif)

# Diagnostics

[Full set of diagnostics](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/Diagnostics.md)