using System.Linq;

class TestClass
{
    private static readonly int[] {|CI0008:Values|} = [1, 2, 3, 4];

    public bool Check(int val)
    {
        return Values.Contains(val);
    }
}
