using System.Linq;

class TestClass
{
    void TestMethod()
    {
        int[] {|CI0008:numbers1|} = [1, 2, 3], numbers2 = [4, 5, 6];
        if (numbers1.Contains(3))
        {
        }
    }
}
