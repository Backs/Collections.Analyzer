using System.Collections.Generic;

class Program
{
    private static int[] GetArray()
    {
        return new[] { 1, 2, 3 };
    }
    
    public List<int> Method()
    {
        var list = new List<int>();
        foreach (var i in GetArray())
        {
            list.Add(i * 2);
        }

        return list;
    }
}