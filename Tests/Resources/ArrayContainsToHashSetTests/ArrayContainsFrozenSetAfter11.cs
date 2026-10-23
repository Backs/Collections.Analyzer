using System.Linq;
using System.Collections.Frozen;

class TestClass
{
    private static readonly FrozenSet<int> Values = new[] { 1, 2, 3, 4 }.ToFrozenSet();

    public bool Check(int val)
    {
        return Values.Contains(val);
    }
}
