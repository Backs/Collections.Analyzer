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
    void TestMethod(MyClass[] list, Guid[] keys)
    {
        foreach (var key in keys)
        {
            var item = {|CI0010:list.SingleOrDefault(x => x.Id == key)|};
            Console.Write(item);
        }
    }
}