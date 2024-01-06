using System.Collections.Generic;

namespace Examples.Lists
{
    public class ListInitializer
    {
        public void Method1()
        {
            var list1 = new List<int> {1, 2, 3, 4};
            var list2 = new List<string>(2) {"1", "2", "3", "4"};
            var list3 = new List<int>(4) {1, 2, 3, 4};

            var t = 2;
            var list4 = new List<int>(t) {1, 2, 3, 4};
            var list5 = new List<int>(new[] {1, 2, 3, 4});
        }
    }
}