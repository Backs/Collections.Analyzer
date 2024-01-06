using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    [ShortRunJob(RuntimeMoniker.Net60)]
    [ShortRunJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class InitListBenchmark
    {
        [BenchmarkCategory("One")]
        [Benchmark(Baseline = true)]
        public List<int> InitList1()
        {
            return new List<int> {1};
        }

        [BenchmarkCategory("One")]
        [Benchmark]
        public List<int> InitListWithSize1()
        {
            return new List<int>(1) {1};
        }

        [BenchmarkCategory("Five")]
        [Benchmark]
        public List<int> InitList5()
        {
            return new List<int> {1, 2, 3, 4, 5};
        }
        
        [BenchmarkCategory("Five")]
        [Benchmark]
        public List<int> InitListWithSize5()
        {
            return new List<int>(5) {1, 2, 3, 4, 5};
        }
    }
}