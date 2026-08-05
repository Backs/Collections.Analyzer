## 0.3.0
- Added [CI0011](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0011.md): Diagnostic to detect potential N+1 Query Problems in loops and LINQ expressions.
- Added support for `.editorconfig` configuration for CI0011 (data access type suffixes, method prefixes, and test method analysis).
- Improved analyzer performance by implementing configuration caching.

## 0.2.15
Added [CI0010](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0010.md): Diagnostic to suggest using `Dictionary` for lookups in collections inside loops or LINQ chains. This optimizes performance from O(N*M) to O(N+M).

## 0.2.14
Added [CI0009](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0009.md): Diagnostic to suggest initialize `List<T>` capacity with the size of a source collection.

## 0.2.13
Added [CI0008](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0008.md): Diagnostic to suggest using HashSet instead of an array for `Contains` operations. `HashSet` provides O(1) lookup performance compared to O(n) for arrays.

## 0.2.12.1
Concurrent collections improvements.

## 0.2.12
Added concurrent collections diagnostic [CI0007](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0007.md)

## 0.2.11
Added collection initializer diagnostic to [CI0006](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0006.md)

## 0.2.8.1
Fix variable initializer.

## 0.2.8
Added variable initializer diagnostic to [CI0003](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0003.md)

## 0.2.7.1:
Fix object initializer diagnostic.

## 0.2.7:

Added object initializer diagnostic to [CI0003](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0003.md)

## 0.2.6:

Added property diagnostic to [CI0003](https://github.com/Backs/Collections.Analyzer/blob/master/Documentation/CI0003.md)
