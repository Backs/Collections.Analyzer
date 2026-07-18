using System.Collections.Generic;

namespace Examples.Lists
{
    public class ListInitializer
    {
        public void Method1()
        {
            var list1 = {|CI0006:new List<int> {1, 2, 3, 4}|};
        }
    }
}