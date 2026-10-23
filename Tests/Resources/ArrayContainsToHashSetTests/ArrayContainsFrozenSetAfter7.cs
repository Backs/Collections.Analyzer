using System.Linq;
using System.Collections.Frozen;

class TestClass
{
    void TestMethod()
    {
        int[] other = [1, 2];
        FrozenSet<int> numbers = ((int[])[.. other, 3, 4, 5]).ToFrozenSet();
        if (numbers.Contains(3))
        {
        }
    }
}
