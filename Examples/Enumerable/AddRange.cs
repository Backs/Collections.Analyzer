using System.Collections.Generic;
using System.Linq;

namespace Examples.Enumerable
{
    public class AddRange
    {
        public void Method1()
        {
            var list = new List<int>();
            IEnumerable<int> array = new int[10];

            list.AddRange(array.ToArray());
        }
        
        public void Method2()
        {
            var list = new List<int>();

            list.AddRange(GetArray().ToArray());
        }

        private static IEnumerable<int> GetArray()
        {
            return new int[10];
        }
    }
}