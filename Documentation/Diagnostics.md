| Id                  | Title                                           | Severity | Description                                                                                         |
|---------------------|-------------------------------------------------|----------|-----------------------------------------------------------------------------------------------------|
| [CI0001](CI0001.md) | Redundant string conversion                     | Warning  | Redundant string conversion to `char[]` or `List<char>`                                             |
| [CI0002](CI0002.md) | Redundant array conversion                      | Warning  | Redundant `ToArray` method call on array                                                            |
| [CI0003](CI0003.md) | Redundant enumerable conversion                 | Warning  | Redundant `ToArray` or `ToList` method call on types that implements `IEnumerable` interface        |
| [CI0004](CI0004.md) | Not optimal `List<char>` constructor usage      | Warning  | String parameter for `List<char>` constructor is better to use with `ToCharArray` call              |
| [CI0005](CI0005.md) | Not optimal count of collection items           | Warning  | It's better to use LINQ method `Count()` instead of calling `ToArray()` and using property `Length` |
| [CI0006](CI0006.md) | Collection initializer is used without capacity | Warning  | Set initial capacity with collection initializer                                                    |
| [CI0007](CI0007.md) | Method `Any()` is used on concurrent collection | Warning  | Use `IsEmpty` property to check empty collection                                                    |
| [CI0008](CI0008.md) | Consider using HashSet for Contains operations  | Warning  | Array is used with `Contains()` method. `HashSet<T>` provides O(1) lookup performance               |
