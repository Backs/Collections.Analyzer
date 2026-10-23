using System.Linq;
using System.Collections.Generic;

class TestClass
{
    void TestMethod()
    {
        int[] other = [1, 2];
        HashSet<int> numbers = [.. other, 3, 4, 5];
        if (numbers.Contains(3))
        {
        }
    }
}
