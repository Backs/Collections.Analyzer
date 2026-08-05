using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        int externalId = 10;
        foreach (var item in items)
        {
            var data = GetData(externalId);
        }
    }

    private string GetData(int id) => id.ToString();
}
