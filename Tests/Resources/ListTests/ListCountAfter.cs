namespace Examples.Enumerable
{
    using System.Linq;

    public class Count
    {
        public void Method2()
        {
            var array = new[] { 1, 2, 3 };

            var result = array.Select(o => o * o).Count();
        }
    }
}