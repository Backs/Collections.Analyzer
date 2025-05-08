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
| Method       | Job                         | Runtime            | N           |             Mean |           Error |         StdDev |    Ratio |  RatioSD |       Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|--------------|-----------------------------|--------------------|-------------|-----------------:|----------------:|---------------:|---------:|---------:|-----------:|---------:|---------:|----------:|------------:|
| **SumArray** | **ShortRun-.NET 6.0**       | **.NET 6.0**       | **1000**    |     **4.204 μs** |   **2.6710 μs** |  **0.1464 μs** | **1.00** | **0.00** | **0.0153** |    **-** |    **-** |  **32 B** |    **1.00** |
| SumArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000        |         4.451 μs |       0.2072 μs |      0.0114 μs |     1.06 |     0.04 |     1.9379 |        - |        - |    4056 B |      126.75 |
|              |                             |                    |             |                  |                 |                |          |          |            |          |          |           |             |
| SumArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000        |         3.892 μs |       0.1969 μs |      0.0108 μs |     1.00 |     0.00 |     0.0153 |        - |        - |      32 B |        1.00 |
| SumArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000        |         4.311 μs |       0.5124 μs |      0.0281 μs |     1.11 |     0.00 |     1.9379 |        - |        - |    4072 B |      127.25 |
|              |                             |                    |             |                  |                 |                |          |          |            |          |          |           |             |
| **SumArray** | **ShortRun-.NET 6.0**       | **.NET 6.0**       | **1000000** | **3,890.951 μs** | **482.0209 μs** | **26.4212 μs** | **1.00** | **0.00** |      **-** |    **-** |    **-** |  **34 B** |    **1.00** |
| SumArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000000     |     5,015.784 μs |     876.0818 μs |     48.0210 μs |     1.29 |     0.01 |   328.1250 | 328.1250 | 328.1250 | 4000163 B |  117,651.85 |
|              |                             |                    |             |                  |                 |                |          |          |            |          |          |           |             |
| SumArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000     |     3,945.666 μs |      70.1074 μs |      3.8428 μs |     1.00 |     0.00 |          - |        - |        - |         - |          NA |
| SumArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000     |     4,856.721 μs |     398.8151 μs |     21.8604 μs |     1.23 |     0.00 |   328.1250 | 328.1250 | 328.1250 | 4002776 B |          NA |

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
| Method            | Job                         | Runtime            | N           |               Mean |               Error |           StdDev |    Ratio |  RatioSD |         Gen0 |         Gen1 |         Gen2 |      Allocated | Alloc Ratio |
|-------------------|-----------------------------|--------------------|-------------|-------------------:|--------------------:|-----------------:|---------:|---------:|-------------:|-------------:|-------------:|---------------:|------------:|
| **AddRangeArray** | **ShortRun-.NET 6.0**       | **.NET 6.0**       | **1000**    |       **235.3 ns** |       **198.57 ns** |     **10.88 ns** | **1.00** | **0.00** |   **1.9379** |        **-** |        **-** |    **3.96 KB** |    **1.00** |
| AddRangeArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000        |           473.9 ns |           352.70 ns |         19.33 ns |     2.01 |     0.02 |       3.8610 |            - |            - |        7.89 KB |        1.99 |
|                   |                             |                    |             |                    |                     |                  |          |          |              |              |              |                |             |
| AddRangeArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000        |           452.3 ns |            52.20 ns |          2.86 ns |     1.00 |     0.00 |       3.8610 |            - |            - |        7.92 KB |        1.00 |
| AddRangeArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000        |           670.6 ns |            68.26 ns |          3.74 ns |     1.48 |     0.02 |       5.7907 |            - |            - |       11.88 KB |        1.50 |
|                   |                             |                    |             |                    |                     |                  |          |          |              |              |              |                |             |
| **AddRangeArray** | **ShortRun-.NET 6.0**       | **.NET 6.0**       | **1000000** | **2,068,387.8 ns** | **1,806,542.26 ns** | **99,022.68 ns** | **1.00** | **0.00** | **195.3125** | **195.3125** | **195.3125** | **3906.37 KB** |    **1.00** |
| AddRangeArrayCopy | ShortRun-.NET 6.0           | .NET 6.0           | 1000000     |     4,445,583.1 ns |     1,528,758.47 ns |     83,796.41 ns |     2.15 |     0.14 |     281.2500 |     281.2500 |     281.2500 |     7812.67 KB |        2.00 |
|                   |                             |                    |             |                    |                     |                  |          |          |              |              |              |                |             |
| AddRangeArray     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000     |     1,737,686.5 ns |       544,444.69 ns |     29,842.85 ns |     1.00 |     0.00 |     486.3281 |     486.3281 |     486.3281 |     7815.65 KB |        1.00 |
| AddRangeArrayCopy | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 1000000     |     4,025,678.1 ns |     2,103,687.04 ns |    115,310.19 ns |     2.32 |     0.11 |     531.2500 |     531.2500 |     531.2500 |     11721.3 KB |        1.50 |

# InitListBenchmark

```
BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.3803/22H2/2022Update)
AMD Ryzen 7 7840H with Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]                      : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  ShortRun-.NET 6.0           : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  ShortRun-.NET 8.0           : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  ShortRun-.NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9195.0), X64 RyuJIT VectorSize=256

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method            | Job                         | Runtime            |      Mean |      Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------|-----------------------------|--------------------|----------:|-----------:|----------:|------:|--------:|-------:|----------:|------------:|
| InitList5         | ShortRun-.NET 6.0           | .NET 6.0           | 22.610 ns | 11.7130 ns | 0.6420 ns |  2.18 |    0.06 | 0.0153 |     128 B |        1.78 |
| InitListWithSize5 | ShortRun-.NET 6.0           | .NET 6.0           |  9.125 ns |  5.2536 ns | 0.2880 ns |  0.88 |    0.03 | 0.0096 |      80 B |        1.11 |
| InitList1         | ShortRun-.NET 6.0           | .NET 6.0           | 10.360 ns |  0.6193 ns | 0.0339 ns |  1.00 |    0.00 | 0.0086 |      72 B |        1.00 |
| InitListWithSize1 | ShortRun-.NET 6.0           | .NET 6.0           |  5.614 ns |  1.1786 ns | 0.0646 ns |  0.54 |    0.01 | 0.0076 |      64 B |        0.89 |
|                   |                             |                    |           |            |           |       |         |        |           |             |
| InitList5         | ShortRun-.NET 8.0           | .NET 8.0           | 23.186 ns | 17.0998 ns | 0.9373 ns |  2.64 |    0.04 | 0.0153 |     128 B |        1.78 |
| InitListWithSize5 | ShortRun-.NET 8.0           | .NET 8.0           |  9.147 ns |  9.7602 ns | 0.5350 ns |  1.04 |    0.04 | 0.0096 |      80 B |        1.11 |
| InitList1         | ShortRun-.NET 8.0           | .NET 8.0           |  8.788 ns |  3.9375 ns | 0.2158 ns |  1.00 |    0.00 | 0.0086 |      72 B |        1.00 |
| InitListWithSize1 | ShortRun-.NET 8.0           | .NET 8.0           |  6.396 ns | 14.3296 ns | 0.7855 ns |  0.73 |    0.08 | 0.0076 |      64 B |        0.89 |
|                   |                             |                    |           |            |           |       |         |        |           |             |
| InitList5         | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 32.461 ns |  7.8378 ns | 0.4296 ns |  2.35 |    0.07 | 0.0216 |     136 B |        1.70 |
| InitListWithSize5 | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 11.722 ns |  4.3472 ns | 0.2383 ns |  0.85 |    0.00 | 0.0140 |      88 B |        1.10 |
| InitList1         | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 13.837 ns |  4.0679 ns | 0.2230 ns |  1.00 |    0.00 | 0.0127 |      80 B |        1.00 |
| InitListWithSize1 | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 |  6.849 ns |  1.0770 ns | 0.0590 ns |  0.50 |    0.01 | 0.0115 |      72 B |        0.90 |

# ConcurrentCollectionsBenchmark
```
BenchmarkDotNet v0.13.8, Windows 10 (10.0.19045.5796/22H2/2022Update)
AMD Ryzen 7 7840H with Radeon 780M Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.203
[Host]                      : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
ShortRun-.NET 6.0           : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
ShortRun-.NET 8.0           : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX2
ShortRun-.NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256

IterationCount=3  LaunchCount=1  WarmupCount=3
```

| Method                      | Job                         | Runtime            | N     |           Mean |          Error |        StdDev |         Median |    Gen0 |   Gen1 | Allocated |
|-----------------------------|-----------------------------|--------------------|-------|---------------:|---------------:|--------------:|---------------:|--------:|-------:|----------:|
| ConcurrentBagAny            | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |     18.3253 ns |      4.7485 ns |     0.2603 ns |     18.2044 ns |       - |      - |         - |
| ConcurrentBagIsEmpty        | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |      8.0343 ns |     11.1700 ns |     0.6123 ns |      7.9539 ns |       - |      - |         - |
| ConcurrentDictionaryAny     | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |  5,866.2944 ns |  6,138.9004 ns |   336.4938 ns |  5,976.8555 ns |       - |      - |         - |
| ConcurrentDictionaryIsEmpty | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |      2.6298 ns |      8.4205 ns |     0.4616 ns |      2.3871 ns |       - |      - |         - |
| ConcurrentQueueAny          | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |     19.9211 ns |      2.3591 ns |     0.1293 ns |     19.9258 ns |       - |      - |         - |
| ConcurrentQueueIsEmpty      | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |      2.2156 ns |      0.4527 ns |     0.0248 ns |      2.2021 ns |       - |      - |         - |
| ConcurrentStackAny          | ShortRun-.NET 6.0           | .NET 6.0           | 10000 | 12,073.1430 ns |  4,268.6161 ns |   233.9773 ns | 12,042.0578 ns |       - |      - |         - |
| ConcurrentStackIsEmpty      | ShortRun-.NET 6.0           | .NET 6.0           | 10000 |      0.0012 ns |      0.0150 ns |     0.0008 ns |      0.0007 ns |       - |      - |         - |
| ConcurrentBagAny            | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |     18.1914 ns |      6.0201 ns |     0.3300 ns |     18.2135 ns |       - |      - |         - |
| ConcurrentBagIsEmpty        | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |      5.3481 ns |      0.0683 ns |     0.0037 ns |      5.3484 ns |       - |      - |         - |
| ConcurrentDictionaryAny     | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |  5,040.7102 ns |    106.2571 ns |     5.8243 ns |  5,039.2662 ns |       - |      - |         - |
| ConcurrentDictionaryIsEmpty | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |      2.6739 ns |      0.0550 ns |     0.0030 ns |      2.6753 ns |       - |      - |         - |
| ConcurrentQueueAny          | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |     18.8592 ns |      0.2473 ns |     0.0136 ns |     18.8538 ns |       - |      - |         - |
| ConcurrentQueueIsEmpty      | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |      2.2195 ns |      0.0332 ns |     0.0018 ns |      2.2185 ns |       - |      - |         - |
| ConcurrentStackAny          | ShortRun-.NET 8.0           | .NET 8.0           | 10000 | 11,897.5113 ns |  5,832.1532 ns |   319.6800 ns | 11,985.6659 ns |       - |      - |         - |
| ConcurrentStackIsEmpty      | ShortRun-.NET 8.0           | .NET 8.0           | 10000 |      0.0011 ns |      0.0215 ns |     0.0012 ns |      0.0010 ns |       - |      - |         - |
| ConcurrentBagAny            | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 | 39,593.4285 ns | 24,036.7265 ns | 1,317.5341 ns | 40,251.9958 ns | 20.8130 | 3.4180 |  131606 B |
| ConcurrentBagIsEmpty        | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |     17.2450 ns |      0.8773 ns |     0.0481 ns |     17.2457 ns |       - |      - |         - |
| ConcurrentDictionaryAny     | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |     15.7370 ns |      4.2832 ns |     0.2348 ns |     15.6298 ns |  0.0102 |      - |      64 B |
| ConcurrentDictionaryIsEmpty | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |  6,154.6664 ns |    263.5827 ns |    14.4479 ns |  6,147.4174 ns |       - |      - |         - |
| ConcurrentQueueAny          | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |     25.7802 ns |      5.4926 ns |     0.3011 ns |     25.7776 ns |  0.0127 |      - |      80 B |
| ConcurrentQueueIsEmpty      | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |      1.6059 ns |      0.2581 ns |     0.0141 ns |      1.5990 ns |       - |      - |         - |
| ConcurrentStackAny          | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |      9.9623 ns |      0.4113 ns |     0.0225 ns |      9.9539 ns |  0.0064 |      - |      40 B |
| ConcurrentStackIsEmpty      | ShortRun-.NET Framework 4.8 | .NET Framework 4.8 | 10000 |      0.0244 ns |      0.1358 ns |     0.0074 ns |      0.0282 ns |       - |      - |         - |
