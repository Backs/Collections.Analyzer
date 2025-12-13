using System.Linq;
using System.Collections.Generic;

class TestClass
{
    void TestMethod()
    {
        var numbers = new HashSet<int>(new[] { 1, 2, 3, 4, 5 });
        if (numbers.Contains(3))
        {
        }
    }
}
