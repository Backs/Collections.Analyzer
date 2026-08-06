using System;
using System.Collections.Generic;
using NUnit.Framework;

public class TestClassWithAttributes
{
    [Test]
    public void TestMethod(IEnumerable<int> items)
    {
        var repository = new MyRepository();
        foreach (var item in items)
        {
            var data = repository.GetData(item);
        }
    }

    public class MyRepository
    {
        public string GetData(int id) => id.ToString();
    }
}
