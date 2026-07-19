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
    
    private static IEnumerable<MyClass> list = new MyClass[10];
    private static IEnumerable<Guid> keys = new Guid[10];
    
    public static void TestMethod()
    {
        foreach (var key in keys)
        {
            var item = {|CI0010:list.First(x => x.Id == key)|};
            Console.Write(item);
        }
    }
}