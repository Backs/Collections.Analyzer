using System.Collections.Generic;

class Program
{
    public List<int> Method(IReadOnlyCollection<int> array)
    {
        var list = {|CI0009:new List<int>()|};
        foreach (var item in array)
        {
            list.Add(item);
        }
        return list;
    }
}