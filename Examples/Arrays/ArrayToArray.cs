using System;
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

        private static int[] GetArray()
        {
            return Array.Empty<int>();
        }

        private static int[] ArrayProperty => Array.Empty<int>();
    }
}