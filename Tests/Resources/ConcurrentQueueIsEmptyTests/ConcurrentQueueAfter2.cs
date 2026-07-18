using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method3()
        {
            return !GetQueue().IsEmpty;
        }
    
        private static ConcurrentQueue<string> GetQueue()
        {
            return new ConcurrentQueue<string>(new[] { "a", "b", "c" });
        }
    }
}