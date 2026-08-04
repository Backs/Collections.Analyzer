using System.Collections.Generic;

class Program
{
    public List<int> Method(int[] array)
    {
        var list = new List<int>(array.Length);
        foreach (var item in array)
        {
            list.Add(item);
        }
        return list;
    }
}