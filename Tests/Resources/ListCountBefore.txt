namespace Examples.Enumerable
{
    using System.Linq;

    public class Count
    {
        public void Method2()
        {
            var array = new[] { 1, 2, 3 };

            var result = {|CI0005:array.Select(o => o * o).ToList().Count|};
        }
    }
}