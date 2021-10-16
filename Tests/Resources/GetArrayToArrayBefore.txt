namespace Examples.Arrays
{
    using System;
    using System.Linq;

    public class ArrayToArray
    {
        public static void ConvertFromMethod()
        {
            var result = {|CI0002:GetArray().ToArray()|};
        }

        private static int[] GetArray()
        {
            return Array.Empty<int>();
        }
    }
}