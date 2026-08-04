using System;
using System.Collections.Generic;
using System.Linq;

class TestClass
{ 
    class MyClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    void TestMethod1()
    {
        var list = new MyClass[10];
        var keys = new Guid[10];

        foreach (var key in keys)
        {
            var item = list.FirstOrDefault(x => x.Id == key);
            Console.Write(item);
        }
    }
    
    void TestMethod2(MyClass[] list, Guid[] keys)
    {
        foreach (var key in keys)
        {
            var item = list.SingleOrDefault(x => x.Id == key);
            Console.Write(item);
        }
    }
    
    private static readonly Guid[] Keys = new Guid[10];
    private static readonly IEnumerable<MyClass> List = new MyClass[10];
    
    public static void TestMethod3()
    {
        foreach (var key in Keys)
        {
            var item = List.First(x => x.Id == key);
            Console.Write(item);
        }
    }
    
    public static void TestMethod4()
    {
        var list = GetData().Where(x => x != null).ToArray();
        
        for (var i = 0; i < Keys.Length; i++)
        {
            var item = list.Single(x => x.Id == Keys[i]);
            Console.Write(item);
        }
    }

    void TestMethod5()
    {
        var list = new MyClass[10];
        var keys = new Guid[10];

        foreach (var key in keys)
        {
            var id = key;
            var item = list.FirstOrDefault(x => x.Id == id);
            Console.Write(item);
        }
    }
    
    void TestMethod6()
    {
        var list = new MyClass[10];
        var keys = new Tuple<Guid, int>[10];

        foreach (var key in keys)
        {
            var item = list.FirstOrDefault(x => x.Id == key.Item1);
            Console.Write(item);
        }
    }
    
    void TestMethod7()
    {
        var list = new MyClass[10];
        var keys = new Tuple<Guid, int>[10];

        var result = keys.Select(x => list.FirstOrDefault(o=>o.Id == x.Item1)).ToArray();
    }
    
    private static IEnumerable<MyClass> GetData()
    {
        return Enumerable.Empty<MyClass>();
    }
}