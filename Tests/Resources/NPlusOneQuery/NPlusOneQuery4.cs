using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestClass
{
    public void TestMethod(IEnumerable<int> items)
    {
        foreach (var item in items)
        {
            Process(item);
        }
    }

    private void Process(int id) { }
    
    public async Task TestMethodAsync(IEnumerable<int> items)
    {
        foreach (var item in items)
        {
            await ProcessAsync(item);
        }
    }
    
    private Task ProcessAsync(int id) => Task.CompletedTask;
}
