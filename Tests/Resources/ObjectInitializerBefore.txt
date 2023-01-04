using System;
using System.Linq;
public class C 
{
    private static void Initialize()
    {
        var value1 = new MyClass
        {
            ArrayProperty1 = Array.Empty<int>(),
            ArrayProperty2 = Array.Empty<string>()
        };

        var value2 = new MyClass
        {
            ArrayProperty1 = {|CI0003:value1.ArrayProperty1.ToArray()|},
            ArrayProperty2 = value1.ArrayProperty2
        };
    }

    private class MyClass
    {
        public int[] ArrayProperty1 { get; set; }
        public string[] ArrayProperty2 { get; set; }
    }
}