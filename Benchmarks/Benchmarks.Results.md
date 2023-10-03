# ArrayToArrayBenchmark

```

BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.3516/22H2/2022Update)
AMD Ryzen 7 4700U with Radeon Graphics, 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.203
  [Host]                      : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  ShortRun-.NET 6.0           : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  ShortRun-.NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method       | Job                         | Runtime            | N       | Mean         | Error       | StdDev     | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------- |---------------------------- |------------------- |-------- |-------------:|------------:|-----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| **SumArray**     | **ShortRun-.NET 6.0**           | **.NET 6.0**           | **1000**    |     **4.204 μs** |   **2.6710 μs** |  **0.1464 μs** |  **1.00** |    **0.00** |   **0.0153** |        **-** |        **-** |      **32 B** |        **1.00** |
| SumArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000    |     4.451 μs |   0.2072 μs |  0.0114 μs |  1.06 |    0.04 |   1.9379 |        - |        - |    4056 B |      126.75 |
|              |                             |                    |         |              |             |            |       |         |          |          |          |           |             |
| SumArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000    |     3.892 μs |   0.1969 μs |  0.0108 μs |  1.00 |    0.00 |   0.0153 |        - |        - |      32 B |        1.00 |
| SumArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000    |     4.311 μs |   0.5124 μs |  0.0281 μs |  1.11 |    0.00 |   1.9379 |        - |        - |    4072 B |      127.25 |
|              |                             |                    |         |              |             |            |       |         |          |          |          |           |             |
| **SumArray**     | **ShortRun-.NET 6.0**           | **.NET 6.0**           | **1000000** | **3,890.951 μs** | **482.0209 μs** | **26.4212 μs** |  **1.00** |    **0.00** |        **-** |        **-** |        **-** |      **34 B** |        **1.00** |
| SumArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000000 | 5,015.784 μs | 876.0818 μs | 48.0210 μs |  1.29 |    0.01 | 328.1250 | 328.1250 | 328.1250 | 4000163 B |  117,651.85 |
|              |                             |                    |         |              |             |            |       |         |          |          |          |           |             |
| SumArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000 | 3,945.666 μs |  70.1074 μs |  3.8428 μs |  1.00 |    0.00 |        - |        - |        - |         - |          NA |
| SumArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000 | 4,856.721 μs | 398.8151 μs | 21.8604 μs |  1.23 |    0.00 | 328.1250 | 328.1250 | 328.1250 | 4002776 B |          NA |

# AddRangeBenchmark

```

BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.3516/22H2/2022Update)
AMD Ryzen 7 4700U with Radeon Graphics, 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.203
  [Host]                      : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  ShortRun-.NET 6.0           : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  ShortRun-.NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method            | Job                         | Runtime            | N       | Mean           | Error           | StdDev        | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|------------------ |---------------------------- |------------------- |-------- |---------------:|----------------:|--------------:|------:|--------:|---------:|---------:|---------:|-----------:|------------:|
| **AddRangeArray**     | **ShortRun-.NET 6.0**           | **.NET 6.0**           | **1000**    |       **235.3 ns** |       **198.57 ns** |      **10.88 ns** |  **1.00** |    **0.00** |   **1.9379** |        **-** |        **-** |    **3.96 KB** |        **1.00** |
| AddRangeArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000    |       473.9 ns |       352.70 ns |      19.33 ns |  2.01 |    0.02 |   3.8610 |        - |        - |    7.89 KB |        1.99 |
|                   |                             |                    |         |                |                 |               |       |         |          |          |          |            |             |
| AddRangeArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000    |       452.3 ns |        52.20 ns |       2.86 ns |  1.00 |    0.00 |   3.8610 |        - |        - |    7.92 KB |        1.00 |
| AddRangeArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000    |       670.6 ns |        68.26 ns |       3.74 ns |  1.48 |    0.02 |   5.7907 |        - |        - |   11.88 KB |        1.50 |
|                   |                             |                    |         |                |                 |               |       |         |          |          |          |            |             |
| **AddRangeArray**     | **ShortRun-.NET 6.0**           | **.NET 6.0**           | **1000000** | **2,068,387.8 ns** | **1,806,542.26 ns** |  **99,022.68 ns** |  **1.00** |    **0.00** | **195.3125** | **195.3125** | **195.3125** | **3906.37 KB** |        **1.00** |
| AddRangeArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000000 | 4,445,583.1 ns | 1,528,758.47 ns |  83,796.41 ns |  2.15 |    0.14 | 281.2500 | 281.2500 | 281.2500 | 7812.67 KB |        2.00 |
|                   |                             |                    |         |                |                 |               |       |         |          |          |          |            |             |
| AddRangeArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000 | 1,737,686.5 ns |   544,444.69 ns |  29,842.85 ns |  1.00 |    0.00 | 486.3281 | 486.3281 | 486.3281 | 7815.65 KB |        1.00 |
| AddRangeArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000 | 4,025,678.1 ns | 2,103,687.04 ns | 115,310.19 ns |  2.32 |    0.11 | 531.2500 | 531.2500 | 531.2500 | 11721.3 KB |        1.50 |
