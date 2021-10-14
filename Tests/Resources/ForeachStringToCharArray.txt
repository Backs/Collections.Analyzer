using System;
namespace Examples
{
    public class Foreach
    {
        public static void RunForeach()
        {
            var str = "string";

            foreach (var c in str.ToCharArray())
            {
                Console.WriteLine(c);
            }
        }
    }
}