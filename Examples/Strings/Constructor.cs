using System.Collections.Generic;
using System.Linq;

namespace Examples.Strings
{
    public class Constructor
    {
        public static void List()
        {
            var str = "string";

            var result = new List<char>(str.ToArray());
        }
    }
}