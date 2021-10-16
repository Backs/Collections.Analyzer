namespace Examples.Arrays
{
    using System;
    using System.Linq;

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

        private static int[] GetArray()
        {
            return Array.Empty<int>();
        }
    }
}