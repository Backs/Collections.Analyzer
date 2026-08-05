using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        var repository = new MyRepository();
        foreach (var item in items)
        {
            var data = repository.FetchData(item);
        }
    }

    public class MyRepository
    {
        public string FetchData(int id) => id.ToString();
    }
}
