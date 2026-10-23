namespace TestNamespace
{
    using System.Linq;
    using System.Collections.Frozen;

    class TestClass
    {
        void TestMethod()
        {
            int[] {|CI0008:numbers|} = [1, 2, 3, 4, 5];
            if (numbers.Contains(3))
            {
            }
        }
    }
}
