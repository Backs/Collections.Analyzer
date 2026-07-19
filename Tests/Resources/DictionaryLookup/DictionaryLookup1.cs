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
    void TestMethod()
    {
        var list = new MyClass[10];
        var keys = new Guid[10];

        foreach (var key in keys)
        {
            var item = {|CI0010:list.FirstOrDefault(x => x.Id == key)|};
            Console.Write(item);
        }
    }
}