using System.Linq;

class TestClass
{
    void TestMethod()
    {
        var {|CI0008:numbers|} = new[] { 1, 2, 3, 4, 5 };
        if (numbers.Contains(3))
        {
        }
    }
}
