using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method3()
        {
            return GetBag().Any();
        }
    
        private static ConcurrentBag<string> GetBag()
        {
            return new ConcurrentBag<string>(new[] { "a", "b", "c" });
        }
    }
}