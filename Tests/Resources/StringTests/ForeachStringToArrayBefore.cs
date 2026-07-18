namespace Examples
{
    using System;
    using System.Linq;

    public class ForeachToArray
    {
        public static void RunForeach()
        {
            var str = "string";

            foreach (var c in {|CI0001:str.ToArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}