using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method2()
        {
            var queue = new ConcurrentQueue<string>(new[] { "a", "b", "c" });
    
            return queue.Any(o => o == "d");
        }
    }
}