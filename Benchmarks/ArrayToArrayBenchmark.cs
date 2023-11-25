using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    [ShortRunJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class ArrayToArrayBenchmark
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
        public int SumArray()
        {
            return array.Sum();
        }

        [Benchmark]
        public int SumArrayCopy()
        {
            return array.ToArray().Sum();
        }
    }
}