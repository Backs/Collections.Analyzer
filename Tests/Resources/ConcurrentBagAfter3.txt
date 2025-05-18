using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections
{
    public class C 
    {
        public void Method6()
        {
            var bag = new ConcurrentBag<string>(new[] { "a", "b", "c" });
    
            WithEmpty(!bag.IsEmpty);
        }
    
        private void WithEmpty(bool b)
        {
            throw new System.NotImplementedException();
        }
    }
}