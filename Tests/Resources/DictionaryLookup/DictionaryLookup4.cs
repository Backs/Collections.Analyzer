using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Resources.DictionaryLookup;

class DictionaryLookup4
{
    public class MyClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    
    private static Guid[] Keys = new Guid[10];
    
    public static void TestMethod4()
    {
        var list = GetData().Where(x => x != null).ToArray();
        
        for (var i = 0; i < Keys.Length; i++)
        {
            var item = {|CI0010:list.Single(x => x.Id == Keys[i])|};
            Console.Write(item);
        }
    }

    public static IEnumerable<MyClass> GetData()
    {
        return Enumerable.Empty<MyClass>();
    }
}