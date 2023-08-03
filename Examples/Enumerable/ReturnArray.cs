using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.Enumerable
{
    public class ReturnArray
    {
        public static IEnumerable<int> Method1()
        {
            IEnumerable<int> list = new List<int>();

            list = list.Select(o => o);

            return list.ToArray();
        }
        
        public static async Task<IEnumerable<int>> Method1Async()
        {
            IEnumerable<int> list = new List<int>();

            list = list.Select(o => o);

            await Task.CompletedTask;
            
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
        
        public static async Task<IEnumerable<int>> Method3Async()
        {
            var list = new HashSet<int>();
            
            await Task.CompletedTask;

            return list.ToList();
        }

        public static IEnumerable<int> Method4()
        {
            return GetSet().ToArray();
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

        public static IEnumerable<int> Property1
        {
            get
            {
                var list = new HashSet<int>();

                return list.ToList();
            }
        }
        
        public static IEnumerable<int> Property2
        {
            get
            {
                var list = GetSet();

                return list.ToList();
            }
        }
    }
}