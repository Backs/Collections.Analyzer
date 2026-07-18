using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class AssignEnumerable
    {
        public void Method1()
        {
            IEnumerable<int> result = GetArray().ToArray();
        }
        
        private static IEnumerable<int> GetArray()
        {
            return new int[10];
        }
    }
}