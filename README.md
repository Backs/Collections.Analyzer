[![MIT](https://img.shields.io/github/license/Backs/CollectionsInspection)](LICENSE)
[![AppVeyor](https://img.shields.io/appveyor/build/Backs/collectionsinspection)](https://ci.appveyor.com/project/Backs/collectionsinspection)
[![Nuget](https://img.shields.io/nuget/v/CollectionsDiagnostic)](https://www.nuget.org/packages/CollectionsDiagnostic/)

Collections Inspection is a set of roslyn-based diagnostics for C#-projects that detect potential problems with operating different collections.

# Motivation
Detailed information is indicated in the following articles:
- [Part 1: string to array of chars](https://blog.rogatnev.net/posts/2021/09/Harmful-collection-transformations-part-1-strings.html)
- [Part 2: automatic diagnostics](https://blog.rogatnev.net/posts/2021/10/Harmful-collection-transformations-part-2-diagnostics.html)

# Features
### Compiler warnings
Analyze your C#-code and warn about redundant method calls.

![Code fix string](https://raw.githubusercontent.com/Backs/CollectionsInspection/master/Documentation/img/string-example-2.png)

### Code fixes
Automatically fixes found problems.
![Code fix enumerable](https://raw.githubusercontent.com/Backs/CollectionsInspection/master/Documentation/img/enumerable-example-1.gif)

# Diagnostics


[Full set of diagnostics](https://github.com/Backs/CollectionsInspection/blob/master/Documentation/Diagnostics.md)