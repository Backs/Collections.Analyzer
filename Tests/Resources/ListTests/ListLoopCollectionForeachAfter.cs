using System.Collections.Generic;

class Program
{
    public List<int> Method(IReadOnlyCollection<int> array)
    {
        var list = new List<int>(array.Count);
        foreach (var item in array)
        {
            list.Add(item);
        }
        return list;
    }
}