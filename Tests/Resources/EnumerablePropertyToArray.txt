using System.Collections.Generic;
using System.Linq;

public class C {
    public static IEnumerable<int> Property1
    {
        get
        {
            var list = new HashSet<int>();

            return list.ToList();
        }
    }
}