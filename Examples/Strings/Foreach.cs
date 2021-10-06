namespace Examples.Strings
{
    using System;
    using System.Linq;

    public class Foreach
    {
        public static void ForeachToArray()
        {
            var str = "string";

            foreach (var c in str.ToArray())
            {
                Console.WriteLine(c);
            }
        }
        
        public static void ForeachToCharArray()
        {
            var str = "string";

            foreach (var c in str.ToCharArray())
            {
                Console.WriteLine(c);
            }
        }
        
        public static void ForeachToList()
        {
            var str = "string";

            foreach (var c in str.ToList())
            {
                Console.WriteLine(c);
            }
        }
    }
}