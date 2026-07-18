using System;
using System.Collections.Generic;
using System.Linq;

public class MyClass
{
    public Guid Id { get; set; }
}

public class TestClass
{
    void TestMethod7()
    {
        var list = new MyClass[10];
        var keys = new Tuple<Guid, int>[10];

        var result = keys.Select(x => list.FirstOrDefault(o => o.Id == x.Item1)).ToArray();
    }
}
