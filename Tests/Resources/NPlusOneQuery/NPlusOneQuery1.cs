using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        var repository = new MyRepository();
        foreach (var item in items)
        {
            var data = {|CI0011:repository.GetData(item)|};
        }
    }

    public class MyRepository
    {
        public string GetData(int id) => id.ToString();
    }
}
