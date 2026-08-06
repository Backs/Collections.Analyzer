using System;
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(int[] items)
    {
        var repository = new MyRepository();
        for (int i = 0; i < items.Length; i++)
        {
            var data = {|CI0011:repository.GetData(i)|};
        }
    }

    public class MyRepository
    {
        public string GetData(int id) => id.ToString();
    }
}
