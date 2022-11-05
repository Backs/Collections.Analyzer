namespace Examples
{
    using System.Collections.Generic;
    using System.Linq;

    public class ReturnArray
    {
        public static IEnumerable<int> Method1()
        {
            var list = new List<int>();

            return list.ToArray();
        }
    }
}