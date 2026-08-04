using System.Collections.Generic;

namespace Examples.Lists
{
    public class ListInitializer
    {
        public void Method1()
        {
            List<int> list1 = {|CI0006:new () {1, 2, 3, 4}|};
        }
    }
}