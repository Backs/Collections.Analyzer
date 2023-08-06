using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples.Arrays
{
    public class ArrayToArray
    {
        public static void Convert()
        {
            var array = Array.Empty<int>();
            var result = array.ToArray();
        }

        public static void ConvertFromMethod()
        {
            var result = GetArray().ToArray();
        }

        public static void ConvertFromProperty()
        {
            var result = ArrayProperty.ToArray();
        }

        private static int[] ArrayProperty => Array.Empty<int>();

        private static int[] GetArray()
        {
            return Array.Empty<int>();
        }

        private static void Initialize()
        {
            var value1 = new MyClass
            {
                ArrayProperty = Array.Empty<int>(),
                EnumerableProperty = Array.Empty<string>()
            };

            var value2 = new MyClass
            {
                ArrayProperty = value1.ArrayProperty.ToArray(),
                EnumerableProperty = value1.EnumerableProperty.ToList()
            };
        }

        private class MyClass
        {
            public int[] ArrayProperty { get; set; } = null!;
            public IEnumerable<string> EnumerableProperty { get; set; } = null!;
        }
    }
}