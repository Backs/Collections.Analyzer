using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method1()
        {
            var stack = new ConcurrentStack<string>(new[] { "a", "b", "c" });
    
            return stack.IsEmpty;
        }
    }
}