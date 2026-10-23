using System.Linq;
using System.Collections.Frozen;

class TestClass
{
    public FrozenSet<int> List3 { get; } = new[] { 1, 2, 3, 4 }.ToFrozenSet();

    public bool Check(int value)
    {
        return List3.Contains(value);
    }
}
