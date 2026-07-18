using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        foreach (var item in items)
        {
            var data = GetList(item);
        }
    }

    private List<string> GetList(int id) => new List<string>();
}
