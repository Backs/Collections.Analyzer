namespace Examples
{
    using System.Collections.Generic;
    using System.Linq;

    public class Constructor
    {
        public static void List()
        {
            var result = {|CI0001:GetString().ToCharArray()|}.Select(o => o);
        }
        private static string GetString()
        {
            return "str";
        }
    }
}