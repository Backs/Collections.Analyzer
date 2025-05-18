using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class C 
    {
        public void Method6()
        {
            var dict = new ConcurrentDictionary<int, string>
                        {
                            [0] = "str"
                        };
    
            WithEmpty({|CI0007:dict.Any|}());
        }
    
        private void WithEmpty(bool b)
        {
            throw new System.NotImplementedException();
        }
    }
}