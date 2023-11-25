using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    [ShortRunJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class AddRangeBenchmark
    {
        [Params(1000, 1000000)]
        public int N { get; set; }

        private int[] array;
        
        [GlobalSetup]
        public void SetUp()
        {
            array = new int[N];
        }

        [Benchmark(Baseline = true)]
        public List<int> AddRangeArray()
        {
            var list = new List<int>();
            list.AddRange(array);
            return list;
        }

        [Benchmark]
        public List<int> AddRangeArrayCopy()
        {
            var list = new List<int>();
            list.AddRange(array.ToArray());
            return list;
        }
    }
}