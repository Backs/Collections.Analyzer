using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class AssignEnumerable
    {
        public void Method2()
        {
            var enumerable = System.Linq.Enumerable.Empty<int>();
            IEnumerable<int> result = {|CI0003:enumerable.ToList()|};
        }
    }
}