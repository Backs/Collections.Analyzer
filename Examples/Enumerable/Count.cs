namespace Examples.Enumerable
{
    using System.Collections.Generic;
    using System.Linq;

    public class Count
    {
        public void Method1()
        {
            var array = new[] { 1, 2, 3 };
            var result = array.Select(o => o * o).Where(o => o < 3).ToArray().Length;
        }
        
        public void Method2()
        {
            var array = new[] { 1, 2, 3 };
            var result = array.Select(o => o * o).ToList().Count;
        }
        
        public void Method3()
        {
            var array = new[] { 1, 2, 3 };

            var result = array.ToList().Count;
        }
        
        public void Method4()
        {
            var array = new[] { 1, 2, 3 };

            var result = array.ToArray().Length;
        }
        
        public void Method5()
        {
            var array = new HashSet<int>(new[] { 1, 2, 3 });
            var result = array.ToArray().Length;
        }
    }
}