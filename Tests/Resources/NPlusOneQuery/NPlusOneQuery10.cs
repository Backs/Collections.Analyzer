using System;
using System.Collections.Generic;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery10
    {
        public void TestMethod(IEnumerable<int> items, IEnumerable<int> otherItems)
        {
            var repository = new MyRepository();
            foreach (var item in items)
            {
                foreach (var other in otherItems)
                {
                    var data = {|CI0011:repository.GetData(item)|};
                    var data2 = {|CI0011:repository.GetData(other)|};
                }
            }
        }

        public class MyRepository
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
