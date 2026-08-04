namespace Examples
{
    using System.Collections.Generic;
    using System.Linq;

    public class SetToArray
    {
        public static IEnumerable<int> Method()
        {
            return GetSet().ToArray();
        }

        public static HashSet<int> GetSet()
        {
            return new HashSet<int>();
        }
    }
}