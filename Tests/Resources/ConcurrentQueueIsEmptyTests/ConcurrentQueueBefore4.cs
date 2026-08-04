using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method1()
        {
            var queue = new ConcurrentQueue<string>(new[] { "a", "b", "c" });
    
            return !{|CI0007:queue.Any|}();
        }
    }
}