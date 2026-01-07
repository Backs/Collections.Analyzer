using System.Collections.Generic;

namespace Examples.Lists;

public class ListCapacity
{
    private readonly List<int> list1 = new() { 1, 2, 3 };
    private List<int> list2 = new List<int> { 1, 2, 3 };

    public List<int> Method1()
    {
        return new List<int> { 1, 2, 3 };
    }

    public List<int> Method2()
    {
        List<int> list = new() { 1, 2, 3 };
        return list;
    }
    
    public List<int> Method3()
    {
        return new List<int>(1) { 1, 2, 3 };
    }
    
    public List<int> Method4()
    {
        const int capacity = 2;
        return new List<int>(capacity) { 1, 2, 3 };
    }
}