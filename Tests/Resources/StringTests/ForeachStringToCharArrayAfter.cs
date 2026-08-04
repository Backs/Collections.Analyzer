using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var value = "string";

            foreach (var c in value)
            {
                Console.WriteLine(c);
            }
        }
    }
}