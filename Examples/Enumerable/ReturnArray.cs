namespace Examples.Enumerable
{
    using System.Collections.Generic;
    using System.Linq;

    public class ReturnArray
    {
        public static IEnumerable<int> Method1()
        {
            IEnumerable<int> list = new List<int>();

            list = list.Select(o => o);

            return list.ToArray();

        }
        
        public static IEnumerable<int> Method2()
        {
            List<int> list = new List<int>();

            return list.ToArray();
        }
        
        public static IEnumerable<int> Method3()
        {
            var list = new List<int>();

            return list.ToArray();
        }
        
        public static int[] Method_t()
        {
            IEnumerable<int> list = new List<int>();

            list = list.Select(o => o);

            return list.ToArray();
        }
    }
}