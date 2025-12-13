using System.Collections.Generic;
using System.Linq;

namespace Examples.HashSets;

public class DistinctToArray
{
    public void Method1()
    {
        var numbers = new[] { 1, 2, 3 };
        ICollection<int> result = numbers.Distinct().ToArray();
    }
}