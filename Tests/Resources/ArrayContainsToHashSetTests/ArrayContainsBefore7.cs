using System.Linq;

class TestClass
{
    void TestMethod()
    {
        int[] other = [1, 2];
        int[] {|CI0008:numbers|} = [.. other, 3, 4, 5];
        if (numbers.Contains(3))
        {
        }
    }
}
