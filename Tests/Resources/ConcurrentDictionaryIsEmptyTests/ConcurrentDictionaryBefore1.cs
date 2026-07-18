using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class DictionaryIsEmpty
    {
        public bool Method()
        {
            var dict = new ConcurrentDictionary<int, string>
            {
                [0] = "str"
            };
    
            return {|CI0007:dict.Any|}();
        }
    }
}