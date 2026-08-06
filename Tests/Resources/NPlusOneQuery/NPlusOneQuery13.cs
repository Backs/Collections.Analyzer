using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery13
    {
        public void TestMethod(IEnumerable<int> items)
        {
            var repository = new MyRepository();
            var data = items.Select(id => 
            {
                var val = {|CI0011:repository.GetData(id)|};
                return val;
            }).ToList();
        }

        public class MyRepository
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
