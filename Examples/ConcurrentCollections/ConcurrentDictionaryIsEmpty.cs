using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentDictionaryIsEmpty
{
    public bool Method1()
    {
        var dict = new ConcurrentDictionary<int, string>
        {
            [0] = "str"
        };

        return dict.Any();
    }

    public bool Method2()
    {
        var dict = new ConcurrentDictionary<int, string>
        {
            [0] = "str"
        };

        return dict.Any(o => o.Value == "str");
    }
    
    public bool Method3()
    {
        return GetDict().Any();
    }

    private static ConcurrentDictionary<int, string> GetDict()
    {
        return new ConcurrentDictionary<int, string>
        {
            [0] = "str"
        };
    }
}