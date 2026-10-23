using System.Linq;
using System.Collections.Frozen;

class TestClass
{
    void TestMethod()
    {
        FrozenSet<int> numbers = new[] { 1, 2, 3, 4, 5 }.ToFrozenSet();
        if (numbers.Contains(3))
        {
        }
    }
}
