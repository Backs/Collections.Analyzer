using System.Collections.Generic;

class Program
{
    public List<int> Method(int[] array)
    {
        var list = {|CI0009:new List<int>()|};
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
        return list;
    }
}