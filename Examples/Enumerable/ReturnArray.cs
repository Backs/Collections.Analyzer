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
            var list = new List<int>();

            return list.ToArray();
        }
        
        public static IEnumerable<int> Method3()
        {
            var list = new HashSet<int>();

            return list.ToList();
        }

        public static IEnumerable<int> Method4()
        {
            return GetSet();
        }

        public static HashSet<int> GetSet()
        {
            return new HashSet<int>();
        }

        public static int[] Method()
        {
            var list = new List<int>();

            return list.ToArray();
        }
        
        public static IReadOnlyCollection<int> Method5()
        {
            var list = new HashSet<int>();

            return list.ToList();
        }
    }
}