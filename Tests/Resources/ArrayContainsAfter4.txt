using System.Linq;
using System.Collections.Generic;

class TestClass
{
    public HashSet<int> List3 { get; } = new HashSet<int>(new[] { 1, 2, 3, 4 });

    public bool Check(int value)
    {
        return List3.Contains(value);
    }
}
