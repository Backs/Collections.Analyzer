using System.Linq;
using System.Collections.Frozen;

class TestClass
{
    private readonly FrozenSet<int> _list2 = new[] { 1, 2, 3, 4 }.ToFrozenSet();

    public bool Method()
    {
        return _list2.Contains(5);
    }
}
