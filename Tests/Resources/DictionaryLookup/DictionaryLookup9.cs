using System.Collections.Generic;
using System.Linq;

public class TestClass9
{
    public void TestMethod9(List<string> source, OtherClass other)
    {
        foreach (var item in source)
        {
            // This should NOT trigger CI0010 because it's a property access
            var found = other.Items.FirstOrDefault(it => it == item);
        }
    }
}

public class OtherClass
{
    public List<string> Items { get; set; }
}
