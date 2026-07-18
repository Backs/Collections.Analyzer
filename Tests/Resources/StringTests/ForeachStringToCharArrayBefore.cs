using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = "string";

            foreach (var c in {|CI0001:value.ToCharArray()|})
            {
                Console.WriteLine(c);
            }
        }
    }
}