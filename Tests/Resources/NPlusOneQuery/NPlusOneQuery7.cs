using System;
using System.Collections.Generic;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery7
    {
        public void TestMethod(IEnumerable<(int Id, string Name)> items)
        {
            var repository = new MyRepository();
            foreach (var (id, name) in items)
            {
                var data = {|CI0011:repository.GetData(id)|};
            }
        }

        public class MyRepository
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
