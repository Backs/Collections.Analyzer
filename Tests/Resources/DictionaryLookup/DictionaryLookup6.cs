using System;
using System.Collections.Generic;
using System.Linq;

public class TestClass
{
    public string[] ReadRCStatisticByYearAndFormType()
    {
        var groupedRCStatistics = new List<Tuple<int, string>>();

        var result = new string[12];
        for (int i = 0; i < 12; i++)
        {
            var month = i + 1;
            var tuple = groupedRCStatistics.FirstOrDefault(p => p.Item1 == month);
            result[i] = tuple != null ? tuple.Item2 : "t";
        }

        return result;
    }
}
