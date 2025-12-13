using System.Linq;

namespace Examples.Arrays
{
    public class ArrayContains
    {
        public int[] List3 { get; } = new[] { 1, 2, 3, 4 };

        private readonly int[] _list2 = { 1, 2, 3, 4 };

        public bool Method1()
        {
            var list1 = new[] { 1, 2, 3, 4 };

            return list1.Contains(5);
        }

        public bool Method2()
        {
            return _list2.Contains(5);
        }

        public bool Method3()
        {
            return List3.Contains(5);
        }

        public bool Method4()
        {
            var list = new int[] { 1, 2, 3, 4 };

            if (list.Contains(2))
            {
                return true;
            }

            return false;
        }
    }
}