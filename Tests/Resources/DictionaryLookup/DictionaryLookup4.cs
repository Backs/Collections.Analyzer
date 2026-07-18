using System;
using System.Collections.Generic;
using System.Linq;

class TestClass
{
    public class MyClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    
    private static Guid[] keys = new Guid[10];
    
    public static void TestMethod()
    {
        var list = GetData().Where(x => x != null).ToArray();
        
        for (int i = 0; i < keys.Length; i++)
        {
            var item = list.Single(x => x.Id == keys[i]);
            Console.Write(item);
        }
    }

    public static IEnumerable<MyClass> GetData()
    {
        return Enumerable.Empty<MyClass>();
    }
}