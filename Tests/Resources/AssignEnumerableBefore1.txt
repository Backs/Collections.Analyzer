using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class AssignEnumerable
    {
        public void Method1()
        {
            IEnumerable<int> result = {|CI0003:GetArray().ToArray()|};
        }

        private static IEnumerable<int> GetArray()
        {
            return new int[10];
        }
    }
}