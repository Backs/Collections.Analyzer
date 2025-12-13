using System.Linq;

class TestClass
{
    public int[] {|CI0008:List3|} { get; } = new[] { 1, 2, 3, 4 };

    public bool Check(int value)
    {
        return List3.Contains(value);
    }
}
