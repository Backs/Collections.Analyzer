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

        public void Method2()
        {
            var enumerable = System.Linq.Enumerable.Empty<int>();
            IEnumerable<int> result = enumerable.ToList();
        }

        private static IEnumerable<int> GetArray()
        {
            return new int[10];
        }
    }
}