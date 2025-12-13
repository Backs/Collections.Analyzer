using System.Linq;
using System.Collections.Generic;

class TestClass
{
    private readonly HashSet<int> _list2 = new HashSet<int> { 1, 2, 3, 4 };

    public bool Method()
    {
        return _list2.Contains(5);
    }
}
