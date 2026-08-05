using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery12
    {
        public void TestMethod(IEnumerable<int> items)
        {
            var repository = new MyRepository();
            var data = items.Select(id => {|CI0011:repository.GetData(id)|}).ToList();
        }

        public class MyRepository
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
