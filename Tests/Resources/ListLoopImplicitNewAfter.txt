using System.Collections.Generic;

class Program
{
    public List<int> Method(int[] array)
    {
        List<int> list = new(array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
        return list;
    }
}