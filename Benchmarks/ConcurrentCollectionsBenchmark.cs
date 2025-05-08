using System.Collections.Concurrent;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    [ShortRunJob(RuntimeMoniker.Net60)]
    [ShortRunJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class ConcurrentCollectionsBenchmark
    {
        [Params(10000)]
        public int N { get; set; }

        private ConcurrentBag<int> bag = new ConcurrentBag<int>();
        private ConcurrentDictionary<int, string> dictionary = new ConcurrentDictionary<int, string>();
        private ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        private ConcurrentStack<int> stack = new ConcurrentStack<int>();

        [GlobalSetup]
        public void SetUp()
        {
            for (int i = 0; i < N; i++)
            {
                bag.Add(i);
                dictionary.TryAdd(i, i.ToString());
                queue.Enqueue(i);
                stack.Push(i);
            }
        }

        [Benchmark]
        public bool ConcurrentBagAny()
        {
            return bag.Any();
        }

        [Benchmark]
        public bool ConcurrentBagIsEmpty()
        {
            return !bag.IsEmpty;
        }
        
        [Benchmark]
        public bool ConcurrentDictionaryAny()
        {
            return dictionary.Any();
        }

        [Benchmark]
        public bool ConcurrentDictionaryIsEmpty()
        {
            return !dictionary.IsEmpty;
        }
        
        [Benchmark]
        public bool ConcurrentQueueAny()
        {
            return queue.Any();
        }

        [Benchmark]
        public bool ConcurrentQueueIsEmpty()
        {
            return !queue.IsEmpty;
        }
        
        [Benchmark]
        public bool ConcurrentStackAny()
        {
            return stack.Any();
        }

        [Benchmark]
        public bool ConcurrentStackIsEmpty()
        {
            return !stack.IsEmpty;
        }
    }
}