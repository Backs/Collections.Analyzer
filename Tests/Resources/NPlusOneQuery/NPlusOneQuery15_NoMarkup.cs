using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        var service = new MyService();
        foreach (var item in items)
        {
            var data = service.GetData(item);
        }
    }

    public class MyService
    {
        public string GetData(int id) => id.ToString();
    }
}
