using System.Linq;
using System.Collections.Generic;

class TestClass
{
    private static readonly HashSet<int> Values = [1, 2, 3, 4];

    public bool Check(int val)
    {
        return Values.Contains(val);
    }
}
