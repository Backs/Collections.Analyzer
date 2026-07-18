using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method3()
        {
            return GetDict().Any();
        }
    
        private static ConcurrentDictionary<int, string> GetDict()
        {
            return new ConcurrentDictionary<int, string>
            {
                [0] = "str"
            };
        }
    }
}