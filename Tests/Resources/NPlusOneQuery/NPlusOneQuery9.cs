using System;
using System.Collections.Generic;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery9
    {
        public void TestMethod(IEnumerable<int> items)
        {
            var repository = new MyRepository();
            foreach (var item in items)
            {
                var data = repository.GetData(10); // Not using loop variable
            }
        }

        public class MyRepository
        {
            public string GetData(int id) => id.ToString();
        }
    }
}
