using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method3()
        {
            return !GetStack().IsEmpty;
        }
    
        private static ConcurrentStack<string> GetStack()
        {
            return new ConcurrentStack<string>(new[] { "a", "b", "c" });
        }
    }
}