using System.Collections.Concurrent;
using System.Linq;

namespace Examples.ConcurrentCollections;

public class ConcurrentQueueIsEmpty
{
    public bool Method1()
    {
        var dict = new ConcurrentQueue<string>(new[] { "a", "b", "c" });

        return dict.Any();
    }

    public bool Method2()
    {
        var dict = new ConcurrentQueue<string>(new[] { "a", "b", "c" });

        return dict.Any(o => o == "d");
    }

    public bool Method3()
    {
        return GetBag().Any();
    }

    private static ConcurrentQueue<string> GetBag()
    {
        return new ConcurrentQueue<string>(new[] { "a", "b", "c" });
    }
}