using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Resources.NPlusOneQuery
{
    public class NPlusOneQuery8
    {
        public async Task TestMethod(IEnumerable<int> items)
        {
            var repository = new MyRepository();
            foreach (var item in items)
            {
                var data = await {|CI0011:repository.GetAsync(item)|};
                var data2 = await {|CI0011:repository.FindAsync(item)|};
            }
        }

        public class MyRepository
        {
            public Task<string> GetAsync(int id) => Task.FromResult(id.ToString());
            public Task<string> FindAsync(int id) => Task.FromResult(id.ToString());
        }
    }
}
