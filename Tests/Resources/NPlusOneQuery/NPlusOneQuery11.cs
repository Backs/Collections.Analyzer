using System;
using System.Collections.Generic;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery11
    {
        public void TestMethod(IEnumerable<int> items)
        {
            var service = new MyService();
            foreach (var item in items)
            {
                var data = service.GetData(item); // Not a repository/reader/writer/handler
            }
        }

        public class MyService
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
